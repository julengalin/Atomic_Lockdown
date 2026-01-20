using UnityEngine;
using UnityEngine.InputSystem;

public class CambiarCanvasPapel : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.Papel;

    public void ToggleCanvas()
    {
        if (canvasObject == null) return;

        bool estabaAbierto = canvasObject.activeSelf;

        if (estabaAbierto)
        {
            canvasObject.SetActive(false);
            if (interactionLock != null) interactionLock.Limpiar();
            return;
        }

        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
            {
                return;
            }
            else if (interactionLock.tipoActual == InteractionType.None)
            {
                interactionLock.Set(tipo);
            }
        }

        canvasObject.SetActive(true);
    }
}
