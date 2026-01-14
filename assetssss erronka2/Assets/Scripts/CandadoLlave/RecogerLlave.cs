using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecogerLlave : MonoBehaviour
{
    public GestionLlave gestionLlave;
    public AbrirCandadoLlave abrirCandadoLlave;

    [SerializeField] Animator animator;

    public GameObject tubo;

    private bool playing = false;

    Plane dragPlane;
    float enter;

    bool encajada = false;
    float zEncajada;
    float xEncajada = 3.34f;

    float yEntrada;
    float salidaDificil = 0.25f;

    bool bloqueada = false;


    Vector3 posicionBloqueo = new Vector3(-2.77f, 0.3547866f, -4.49f);
    float yTolerancia = 0.02f;

    private void Start()
    {
        Vector3 tp = tubo.transform.localPosition;
        tp.z = 0.00613f;
        tubo.transform.localPosition = tp;

        Vector3 tr = tubo.transform.localEulerAngles;
        tr.z = 0f;
        tubo.transform.localEulerAngles = tr;


    }

    private void OnMouseDown()
    {
        if (!playing) gestionLlave.recogerLlave();

        Camera cam = Camera.main;
        dragPlane = new Plane(-cam.transform.forward, transform.position);

        if (Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 p = ray.GetPoint(enter);

            if (encajada)
            {
                p.x = xEncajada;
                p.z = zEncajada;
            }

            if (!bloqueada)
                transform.position = p;
        }
    }

    public void inGame()
    {
        playing = true;
    }

    public void exitGame()
    {
        playing = false;
    }

    void OnMouseDrag()
    {
        if (!playing) return;
        if (Mouse.current == null) return;

        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (dragPlane.Raycast(ray, out enter))
        {
            Vector3 p = ray.GetPoint(enter);

            if (encajada)
            {
                p.x = xEncajada;
                p.z = zEncajada;
            }

            if (!bloqueada)
                transform.position = p;

            if (!bloqueada && encajada && Mathf.Abs(transform.position.y - posicionBloqueo.y) <= yTolerancia)
            {
                BloquearYLanzarEventos();
            }
        }
    }

    private void BloquearYLanzarEventos()
    {
        bloqueada = true;

        transform.position = posicionBloqueo;


        StartCoroutine(AbrirAnimacion());

    }

    IEnumerator AbrirAnimacion()
    {
        animator.Play("Abrir", 0, 0f);

        yield return null;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        gestionLlave.llaveUsada();
        abrirCandadoLlave.salir();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entra");
        if (other.CompareTag("KeySocket"))
        {
            encajada = true;
            zEncajada = transform.position.z;
            yEntrada = transform.position.y;

            Vector3 p = transform.position;
            p.x = xEncajada;
            p.z = zEncajada;
            transform.position = p;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KeySocket"))
        {
            if (Mathf.Abs(transform.position.y - yEntrada) > salidaDificil)
            {
                encajada = false;
            }
        }
    }
}
