using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RecogerTarjeta : MonoBehaviour
{
    [SerializeField] GestionTarjeta gestionTarjeta;
    [SerializeField] GameObject tarjetaObj;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.None;

    private XRSimpleInteractable interactable;

    void Awake()
    {
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
        if (gestionTarjeta == null) return;

        if (interactionLock != null && tipo != InteractionType.None)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        if (gestionTarjeta.TieneTarjeta())
        {
            if (interactionLock != null && tipo != InteractionType.None)
                interactionLock.Limpiar();
            return;
        }

        gestionTarjeta.RecogerTarjeta();

        if (tarjetaObj != null) tarjetaObj.SetActive(false);
        else gameObject.SetActive(false);

        if (interactionLock != null && tipo != InteractionType.None)
            interactionLock.Limpiar();
    }
}
