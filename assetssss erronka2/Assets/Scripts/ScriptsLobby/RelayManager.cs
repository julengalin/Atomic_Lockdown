using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance { get; private set; }

    [Header("Relay")]
    [SerializeField] private int maxConnections = 4;
    [SerializeField] private string connectionType = "dtls"; // recomendado

    public string LastJoinCode { get; private set; } = "";

    private bool _ready;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitServices();
    }

    public async Task InitServices()
    {
        if (_ready) return;

        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _ready = true;
            Debug.Log("UGS Ready (Auth + Relay).");
        }
        catch (Exception e)
        {
            Debug.LogError($"UGS Init failed: {e}");
            _ready = false;
        }
    }

    public async Task<string> CreateRelayAndStartHost()
    {
        if (!_ready) await InitServices();
        if (!_ready) return "";

        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, connectionType));

            LastJoinCode = joinCode;

            NetworkManager.Singleton.StartHost();
            Debug.Log($"Host started via Relay. Join Code: {joinCode}");

            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogError($"CreateRelay failed: {e}");
            return "";
        }
    }

    public async Task<bool> JoinRelayAndStartClient(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode)) return false;
        if (!_ready) await InitServices();
        if (!_ready) return false;

        try
        {
            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode.Trim());

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAlloc, connectionType));

            bool ok = NetworkManager.Singleton.StartClient();
            Debug.Log($"Client start via Relay: {ok}");
            return ok;
        }
        catch (Exception e)
        {
            Debug.LogError($"JoinRelay failed: {e}");
            return false;
        }
    }
}
