using UnityEngine;

public class RecogerTarjeta : MonoBehaviour
{
    [SerializeField] GestionTarjeta gestionTarjeta;
    [SerializeField] GameObject tarjetaObj;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.None;

    void OnMouseDown()
    {
        if (gestionTarjeta == null) return;

        if (interactionLock != null && tipo != InteractionType.None)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        if (gestionTarjeta.TieneTarjeta()) return;

        gestionTarjeta.RecogerTarjeta();

        if (tarjetaObj != null) tarjetaObj.SetActive(false);
        else gameObject.SetActive(false);

        if (interactionLock != null && tipo != InteractionType.None)
            interactionLock.Limpiar();
    }
}
