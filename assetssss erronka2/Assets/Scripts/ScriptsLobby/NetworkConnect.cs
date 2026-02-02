using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class NetworkConnect : MonoBehaviour
{
    public int maxConnection = 2;
    public UnityTransport transport;

    private Lobby currentLobby;
    private float heartBeatTimer;
    private float lobbyPollTimer;

    [Header("UI (Canvas)")]
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private string nextSceneName = "SampleScene"; // nombre exacto

    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text joinCodeText;
    public TMP_Text playersCountText;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        if (createButton != null) createButton.onClick.AddListener(Create);
        if (joinButton != null) joinButton.onClick.AddListener(Join);

        if (startGameButton != null)
        {
            startGameButton.gameObject.SetActive(false);
            startGameButton.onClick.AddListener(StartGame);
        }

        // Suscribirse a eventos de Netcode (contador REAL de jugadores conectados)
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdatePlayersUIFromNetcode();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        UpdatePlayersUIFromNetcode();
    }

    private void UpdatePlayersUIFromNetcode()
    {
        if (NetworkManager.Singleton == null) return;

        int count = 0;

        // En host/servidor esto es lo más fiable
        if (NetworkManager.Singleton.IsServer)
            count = NetworkManager.Singleton.ConnectedClientsList.Count;
        else
            count = NetworkManager.Singleton.ConnectedClients.Count;

        if (playersCountText != null)
            playersCountText.text = $"Jugadores: {count}/2";

        if (startGameButton != null)
        {
            bool isHost = NetworkManager.Singleton.IsHost;
            bool twoPlayers = count >= 2;
            startGameButton.gameObject.SetActive(isHost && twoPlayers);
        }
    }

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        NetworkManager.Singleton.SceneManager.LoadScene(
            nextSceneName,
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    public async void Create()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        if (joinCodeText != null) joinCodeText.text = newJoinCode;
        Debug.Log("Join code = " + newJoinCode);

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
            {
                { "JOIN_CODE", new DataObject(DataObject.VisibilityOptions.Public, newJoinCode) }
            }
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("Lobby Name", maxConnection, lobbyOptions);

        NetworkManager.Singleton.StartHost();
        UpdatePlayersUIFromNetcode(); // <- importante

        var uiState = FindFirstObjectByType<LobbyUIState>();
        if (uiState != null)
            uiState.SetJoinCodeServer(newJoinCode);
    }

    public async void Join()
    {
        string code = joinCodeInput != null ? joinCodeInput.text.Trim() : "";

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
        UpdatePlayersUIFromNetcode(); // <- importante
    }

    private async void Update()
    {
        // Heartbeat (solo host)
        if (heartBeatTimer > 15f)
        {
            heartBeatTimer -= 15f;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
        heartBeatTimer += Time.deltaTime;

        // Puedes seguir refrescando lobby si quieres (pero NO lo uses para StartButton)
        lobbyPollTimer += Time.deltaTime;
        if (currentLobby != null && lobbyPollTimer > 2f)
        {
            lobbyPollTimer = 0f;
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            // (Opcional) si quieres mostrar el lobby count, pero yo prefiero el real:
            // if (playersCountText != null)
            //     playersCountText.text = $"Jugadores: {currentLobby.Players.Count}/2";
        }

        // Asegura refresco continuo del UI por si acaso
        UpdatePlayersUIFromNetcode();
    }
}
