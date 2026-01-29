using UnityEngine;

public class ForceIdleOnStart : MonoBehaviour
{
    public Animator animator;
    public string idleStateName = "Neutral Idle"; // pon EXACTO el nombre del estado idle

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();

        if (!animator)
        {
            Debug.LogError("[ForceIdleOnStart] No hay Animator.");
            return;
        }

        // Limpia cualquier estado previo
        animator.Rebind();
        animator.Update(0f);

        // Fuerza Idle
        animator.Play(idleStateName, 0, 0f);
        animator.Update(0f);
    }
}
