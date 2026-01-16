using UnityEngine;

public class AbrirPuerta : MonoBehaviour
{

    [SerializeField] Animator animator;

    [SerializeField] private bool seAbre = false;

    private void Update()
    {
        if (seAbre) abrir();
    }

    public void abrir()
    {
        if (seAbre && animator != null)
        {
            animator.Play("Abrir");
        }
    }

    public void setSeAbre()
    {
        seAbre = true;
    }

}
