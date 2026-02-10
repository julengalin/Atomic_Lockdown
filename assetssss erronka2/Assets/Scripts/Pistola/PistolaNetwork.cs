using UnityEngine;
using Unity.Netcode;

public class PistolaNetwork : NetworkBehaviour
{
    public NetworkVariable<ulong> ownerClientId =
        new(ulong.MaxValue, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Cliente llama a esto (helper)
    public void RequestGrab()
    {
        if (NetworkManager.Singleton == null) return;
        RequestGrabRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void RequestRelease()
    {
        RequestReleaseRpc();
    }

    // ✅ NUEVO: Rpc en vez de ServerRpc + RequireOwnership
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestGrabRpc(ulong clientId)
    {
        if (ownerClientId.Value != ulong.MaxValue) return; // ya cogida

        ownerClientId.Value = clientId;

        var no = GetComponent<NetworkObject>();
        if (no != null)
            no.ChangeOwnership(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestReleaseRpc()
    {
        ownerClientId.Value = ulong.MaxValue;

        var no = GetComponent<NetworkObject>();
        if (no != null)
            no.RemoveOwnership();
    }
}
