using UnityEngine;

public class ActivarEmergencia : MonoBehaviour
{
    [SerializeField] DoorEyeLock[] puertas;
    public static bool EmergenciaActiva;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.None;

    bool activada;

    void OnMouseDown()
    {
        if (activada) return;

        if (interactionLock != null && tipo != InteractionType.None)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        activada = true;
        EmergenciaActiva = true;

        if (puertas != null)
        {
            for (int i = 0; i < puertas.Length; i++)
            {
                if (puertas[i] != null)
                    puertas[i].ActivarEmergencia();
            }
        }

        if (interactionLock != null && tipo != InteractionType.None)
            interactionLock.Limpiar();
    }
}
