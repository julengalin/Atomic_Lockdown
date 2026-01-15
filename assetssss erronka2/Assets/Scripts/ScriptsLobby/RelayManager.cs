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
    [SerializeField] private string connectionType = "dtls"; // prueba "udp" si se queda en JOINING

    public string LastJoinCode { get; private set; } = "";

    private bool _ready;
    private bool _netcodeHooksSet;
    private bool _approvalHookSet;

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

            HookNetcodeDebug();   // ✅ logs
            HookApproval();       // ✅ spawn pos/rot desde payload
        }
        catch (Exception e)
        {
            Debug.LogError("UGS Init failed:");
            Debug.LogException(e);
            _ready = false;
        }
    }

    private void HookNetcodeDebug()
    {
        if (_netcodeHooksSet) return;

        var nm = NetworkManager.Singleton;
        if (nm == null)
        {
            Debug.LogError("RelayManager: NetworkManager.Singleton es null (no existe en escena?)");
            return;
        }

        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientConnectedCallback += OnClientConnected;

        nm.OnClientDisconnectCallback -= OnClientDisconnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;

        nm.OnTransportFailure -= OnTransportFailure;
        nm.OnTransportFailure += OnTransportFailure;

        _netcodeHooksSet = true;
        Debug.Log("RelayManager: Netcode callbacks hooked.");
    }

    private void HookApproval()
    {
        if (_approvalHookSet) return;

        var nm = NetworkManager.Singleton;
        if (nm == null)
        {
            Debug.LogError("RelayManager: No NetworkManager para hook approval.");
            return;
        }

        nm.ConnectionApprovalCallback -= ApprovalCheck;
        nm.ConnectionApprovalCallback += ApprovalCheck;

        _approvalHookSet = true;
        Debug.Log("RelayManager: ConnectionApprovalCallback hooked.");
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        // Defaults por si el cliente no manda payload
        Vector3 p = Vector3.zero;
        Quaternion r = Quaternion.identity;

        try
        {
            var data = req.Payload;

            // 7 floats = 28 bytes
            if (data != null && data.Length >= 28)
            {
                int o = 0;

                float ReadFloat()
                {
                    float f = BitConverter.ToSingle(data, o);
                    o += 4;
                    return f;
                }

                p = new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
                r = new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
            }
            else
            {
                Debug.LogWarning($"[APPROVAL] client={req.ClientNetworkId} payload null/short -> spawn en 0");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[APPROVAL] error leyendo payload: {e.Message}");
        }

        // ✅ Aprobar y spawnear player con posición/rotación
        res.Approved = true;
        res.CreatePlayerObject = true;

        res.Position = p;
        res.Rotation = r;

        Debug.Log($"[APPROVAL] client={req.ClientNetworkId} spawnPos={p} spawnRot={r.eulerAngles}");
    }

    private void OnClientConnected(ulong clientId)
    {
        var nm = NetworkManager.Singleton;
        Debug.Log($"✅ NETCODE CONNECTED: clientId={clientId} local={nm.LocalClientId} isServer={nm.IsServer} isClient={nm.IsClient}");
    }

    private void OnClientDisconnected(ulong clientId)
    {
        var nm = NetworkManager.Singleton;
        Debug.LogWarning($"❌ NETCODE DISCONNECTED: clientId={clientId} local={nm.LocalClientId} isServer={nm.IsServer} isClient={nm.IsClient}");
    }

    private void OnTransportFailure()
    {
        Debug.LogError("🚨 TRANSPORT FAILURE (UTP/Relay)");
    }

    public async Task<string> CreateRelayAndStartHost()
    {
        if (!_ready) await InitServices();
        if (!_ready) return "";

        try
        {
            HookNetcodeDebug();
            HookApproval();

            Debug.Log("CreateRelay: creando allocation...");
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            Debug.Log("CreateRelay: pidiendo join code...");
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("CreateRelay: No hay UnityTransport en el NetworkManager.");
                return "";
            }

            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, connectionType));
            Debug.Log($"CreateRelay: RelayServerData aplicado (type={connectionType})");

            LastJoinCode = joinCode;

            bool started = NetworkManager.Singleton.StartHost();
            Debug.Log($"StartHost() returned: {started}");
            Debug.Log($"Host started via Relay. Join Code: {joinCode}");

            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogError("CreateRelay failed:");
            Debug.LogException(e);
            return "";
        }
    }

    public async Task<bool> JoinRelayAndStartClient(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            Debug.LogError("JoinRelay: joinCode vacío");
            return false;
        }

        if (!_ready) await InitServices();
        if (!_ready) return false;

        try
        {
            HookNetcodeDebug();

            string code = joinCode.Trim();
            Debug.Log($"JoinRelay: intentando unirse con code={code}");

            JoinAllocation joinAlloc = await RelayService.Instance.JoinAllocationAsync(code);
            Debug.Log("JoinRelay: JoinAllocation OK");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("JoinRelay: No hay UnityTransport en el NetworkManager.");
                return false;
            }

            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAlloc, connectionType));
            Debug.Log($"JoinRelay: RelayServerData aplicado (type={connectionType})");

            // ✅ Enviar pos/rot inicial al host
            var rig = VRRigReferences.Singleton;
            Vector3 p = rig != null && rig.root != null ? rig.root.position : Vector3.zero;
            Quaternion r = rig != null && rig.root != null ? rig.root.rotation : Quaternion.identity;

            byte[] payload = new byte[sizeof(float) * (3 + 4)];
            int o = 0;

            void WriteFloat(float f)
            {
                var b = BitConverter.GetBytes(f);
                Buffer.BlockCopy(b, 0, payload, o, 4);
                o += 4;
            }

            WriteFloat(p.x); WriteFloat(p.y); WriteFloat(p.z);
            WriteFloat(r.x); WriteFloat(r.y); WriteFloat(r.z); WriteFloat(r.w);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;
            Debug.Log($"[CLIENT PAYLOAD] sendPos={p} sendRot={r.eulerAngles}");

            bool ok = NetworkManager.Singleton.StartClient();
            Debug.Log($"StartClient() returned: {ok}");

            return ok;
        }
        catch (Exception e)
        {
            Debug.LogError("JoinRelay failed:");
            Debug.LogException(e);
            return false;
        }
    }
}
