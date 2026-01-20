using UnityEngine;

public class InteractionLock : MonoBehaviour
{
    public InteractionType tipoActual = InteractionType.None;

    public void Set(InteractionType tipo)
    {
        tipoActual = tipo;
    }

    public void Limpiar()
    {
        tipoActual = InteractionType.None;
    }
}
