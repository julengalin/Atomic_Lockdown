using UnityEngine;

public class LectorTarjeta : MonoBehaviour
{
    [SerializeField] GestionTarjeta gestionTarjeta;
    [SerializeField] Animator animator;

    [SerializeField] string animAbrir = "Abrir";
    [SerializeField] string animError = "Error";
    [SerializeField] string animAbierto = "Abierto";

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.Tarjeta;

    [SerializeField] SlidingDoor door;

    public bool usado = false;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInParent<Animator>();
    }

    void OnMouseDown()
    {
        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        if (usado)
        {
            if (animator != null)
                animator.Play(animAbierto);

            if (interactionLock != null)
                interactionLock.Limpiar();

            return;
        }

        if (gestionTarjeta != null && gestionTarjeta.TieneTarjeta())
        {
            if (animator != null)
                animator.Play(animAbrir);

            gestionTarjeta.ConsumirTarjeta();
            usado = true;
        }
        else
        {
            if (animator != null)
                animator.Play(animError);
        }

        if (interactionLock != null)
            interactionLock.Limpiar();
    }

    public void AbrirPuerta()
    {
        if (door != null)
            door.ForceOpen();
    }
}
