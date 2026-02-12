using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class RecogerTarjeta : MonoBehaviour
{
    [SerializeField] private GestionTarjeta gestionTarjeta;
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
        gestionTarjeta.RecogerTarjeta();

    }
}
