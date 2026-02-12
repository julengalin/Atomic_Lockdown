using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class LectorTarjeta : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private string animAbrir = "Abrir";
    [SerializeField] private string animError = "Error";
    [SerializeField] private string animAbierto = "Abierto";

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.Tarjeta;

    [SerializeField] private SlidingDoor door;

    private NetworkVariable<bool> usado = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private XRSimpleInteractable interactable;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInParent<Animator>();

        interactable = GetComponent<XRSimpleInteractable>();
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Lock local (solo UX)
        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        UseReaderRpc(NetworkManager.Singleton.LocalClientId);

        if (interactionLock != null)
            interactionLock.Limpiar();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void UseReaderRpc(ulong clientId)
    {
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
            return;

        var playerObj = client.PlayerObject;
        if (playerObj == null) return;

        var tarjeta = FindFirstObjectByType<GestionTarjeta>();
        if (tarjeta == null) return;

        if (tarjeta.TieneTarjeta(clientId))
        {
            tarjeta.ConsumirTarjeta();
            usado.Value = true;
            PlayAnimRpc(animAbrir);
            AbrirPuertaServer();
        }
        else
        {
            PlayAnimRpc(animError);
        }

        if (usado.Value)
        {
            PlayAnimRpc(animAbierto);
            return;
        }

        else
        {
            PlayAnimRpc(animError);
        }
    }

    private void AbrirPuertaServer()
    {
        if (!IsServer) return;
        if (door != null)
            door.ForceOpen();
    }

    // Para animaciones/FX en todos
    [Rpc(SendTo.Everyone)]
    private void PlayAnimRpc(string animName)
    {
        if (animator != null && !string.IsNullOrEmpty(animName))
            animator.Play(animName);
    }

    // Si aún quieres llamar esto desde un Animation Event:
    public void AbrirPuerta()
    {
        // Solo el server debe abrir
        if (!IsServer)
        {
            AbrirPuertaRpc();
            return;
        }

        AbrirPuertaServer();
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void AbrirPuertaRpc()
    {
        AbrirPuertaServer();
    }
}
