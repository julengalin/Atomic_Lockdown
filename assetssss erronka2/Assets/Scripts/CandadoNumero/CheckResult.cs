using UnityEngine;

public class CheckResult : MonoBehaviour
{
    [SerializeField] private JuegoCandado juegoCandado;
    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoNumerico;

    void OnMouseDown()
    {
        if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
        {
            return;
        }
        else if (interactionLock.tipoActual == InteractionType.None)
        {
            interactionLock.Set(tipo);
        }
        juegoCandado.check();
    }
}
