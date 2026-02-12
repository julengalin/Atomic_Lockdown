using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class GestionTarjeta : NetworkBehaviour
{
    [SerializeField] private Image iconoTarjeta;        // UI local
    [SerializeField] private GameObject tarjetaVisual;  // SOLO la malla/visual (no el root con NetworkObject)

    private NetworkVariable<long> ownerClientId = new(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Compatibilidad
    public bool TieneTarjeta()
    {
        if (NetworkManager.Singleton == null) return false;
        return TieneTarjeta(NetworkManager.Singleton.LocalClientId);
    }

    public bool TieneTarjeta(ulong clientId)
    {
        return ownerClientId.Value == (long)clientId;
    }

    public void RecogerTarjeta()
    {
        if (NetworkManager.Singleton == null) return;
        PickUpRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void ConsumirTarjeta()
    {
        if (NetworkManager.Singleton == null) return;
        ConsumeRpc(NetworkManager.Singleton.LocalClientId);
    }

    public bool EstaCogidaPorAlguien() => ownerClientId.Value != -1;

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void PickUpRpc(ulong clientId)
    {
        if (ownerClientId.Value != -1) return;

        ownerClientId.Value = (long)clientId;

        // ✅ Ocultamos el VISUAL para todos (RPC)
        SetWorldVisibleRpc(false);

        // ✅ UI solo para el cliente correcto
        ShowIconRpc(clientId);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void ConsumeRpc(ulong clientId)
    {
        if (ownerClientId.Value != (long)clientId) return;

        ownerClientId.Value = -1;

        HideIconRpc(clientId);

        // Si no quieres que reaparezca nunca, NO lo vuelvas a mostrar
        // SetWorldVisibleRpc(true);
    }

    [Rpc(SendTo.Everyone)]
    private void SetWorldVisibleRpc(bool visible)
    {
        if (tarjetaVisual != null)
        {
            tarjetaVisual.SetActive(visible);

            var interactable = tarjetaVisual.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            if (interactable != null)
                interactable.enabled = visible;
        }
    }
    [Rpc(SendTo.Everyone)]
    private void ShowIconRpc(ulong clientId)
    {
        if (NetworkManager.Singleton == null) return;
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (iconoTarjeta != null)
            iconoTarjeta.gameObject.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    private void HideIconRpc(ulong clientId)
    {
        if (NetworkManager.Singleton == null) return;
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (iconoTarjeta != null)
            iconoTarjeta.gameObject.SetActive(false);
    }
}
