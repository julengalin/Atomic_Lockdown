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

    // Si tu JoinCodeInput es TMP_InputField:
    [SerializeField] private TMP_InputField joinCodeInput;

    // Si tu JoinCodeText es TMP_Text / TextMeshProUGUI:
    [SerializeField] private TMP_Text joinCodeText;

    public TMP_Text playersCountText;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Conectar botones a tus métodos existentes
        if (createButton != null) createButton.onClick.AddListener(Create);
        if (joinButton != null) joinButton.onClick.AddListener(Join);
    }

    public async void Create()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // Mostrar el código en tu JoinCodeText
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
    }

    public async void Join()
    {
        // Leer el código desde JoinCodeInput
        string code = joinCodeInput != null ? joinCodeInput.text.Trim() : "";

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(code);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }

    private async void Update()
    {
        // tu heartbeat (corrige el ; que tienes)
        if (heartBeatTimer > 15f)
        {
            heartBeatTimer -= 15f;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
        heartBeatTimer += Time.deltaTime;

        // refrescar lobby para ver jugadores
        lobbyPollTimer += Time.deltaTime;
        if (currentLobby != null && lobbyPollTimer > 2f)
        {
            lobbyPollTimer = 0f;
            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

            if (playersCountText != null)
                playersCountText.text = $"Jugadores: {currentLobby.Players.Count}/2";
        }
    }

}
