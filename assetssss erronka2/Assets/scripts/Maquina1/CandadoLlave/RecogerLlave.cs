using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecogerLlave : MonoBehaviour
{
    public GestionLlave gestionLlave;
    public AbrirCandadoLlave abrirCandadoLlave;

    [SerializeField] Animator animator;

    Transform keySocket;

    Vector3 bloqueoLocalOffset;


    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoLlave;

    public ControlMaquina1 controlMaquina;

    private void OnMouseDown()
    {
        metodoClick();
    }

    public void metodoClick()
    {
        Debug.Log("Click");
        gestionLlave.recogerLlave();

        return;
    } 

    public void BloquearYLanzarEventos()
    {

        if (keySocket != null)
            transform.position = keySocket.position + bloqueoLocalOffset;

        StartCoroutine(AbrirAnimacion());
    }

    IEnumerator AbrirAnimacion()
    {
        animator.Play("Abrir", 0, 0f);

        yield return null;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        gestionLlave.llaveUsada();
        controlMaquina.SetRojo();
    }
}