using Unity.Netcode;
using Unity.Collections;
using TMPro;
using UnityEngine;

public class LobbyUIState : NetworkBehaviour
{
    [Header("UI (Canvas)")]
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private TMP_Text playersCountText;

    public NetworkVariable<FixedString64Bytes> JoinCode =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<int> PlayerCount =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        JoinCode.OnValueChanged += (_, v) =>
        {
            if (joinCodeText) joinCodeText.text = v.ToString();
        };

        PlayerCount.OnValueChanged += (_, v) =>
        {
            if (playersCountText) playersCountText.text = $"{v}/2";
        };

        // Pintar valores actuales al aparecer
        if (joinCodeText) joinCodeText.text = JoinCode.Value.ToString();
        if (playersCountText) playersCountText.text = $"{PlayerCount.Value}/2";

        // Solo el server controla el contador, sin Update
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientCountChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientCountChanged;
            UpdatePlayerCount();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientCountChanged;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientCountChanged;
        }
    }

    private void OnClientCountChanged(ulong _)
    {
        UpdatePlayerCount();
    }

    private void UpdatePlayerCount()
    {
        // Host + clientes conectados
        PlayerCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
    }

    // Llamar SOLO desde el host cuando crea el join code
    public void SetJoinCodeServer(string code)
    {
        if (!IsServer) return;
        JoinCode.Value = code;
    }
}
