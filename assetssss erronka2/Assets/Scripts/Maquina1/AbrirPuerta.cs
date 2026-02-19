using UnityEngine;

public class AbrirPuerta : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] InteractionLock interactionLock;
    public InteractionType tipo;
    [SerializeField] Camera camAnim;

    bool abierto;

    public void Abrir()
    {
        if (abierto || animator == null) return;

        interactionLock.Set(InteractionType.Cinematica);

        animator.Play("Abrir");
        abierto = true;
    }

    public void ReactivarCamarasJugador()
    {
        interactionLock.Limpiar();
    }
}