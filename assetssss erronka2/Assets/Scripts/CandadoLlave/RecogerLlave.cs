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

    Transform keySocket;

    Vector3 bloqueoLocalOffset;
    float yTolerancia = 0.02f;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoLlave;

    public ControlMaquina controlMaquina;

    private void Start()
    {
        Vector3 tp = tubo.transform.localPosition;
        tp.z = 0.00613f;
        tubo.transform.localPosition = tp;

        Vector3 tr = tubo.transform.localEulerAngles;
        tr.z = 0f;
        tubo.transform.localEulerAngles = tr;
    }

    Camera GetCam()
    {
        Camera cam = null;
        if (abrirCandadoLlave != null && abrirCandadoLlave.cam != null) cam = abrirCandadoLlave.cam;
        if (cam == null) cam = Camera.main;
        return cam;
    }

    Vector3 GetPlanePoint()
    {
        if (keySocket != null) return keySocket.position;
        return transform.position;
    }

    void RecalcularPlano(Camera cam)
    {
        Vector3 planePoint = GetPlanePoint();
        dragPlane = new Plane(-cam.transform.forward, planePoint);
    }

    private void OnMouseDown()
    {
        if (!playing)
        {
            gestionLlave.recogerLlave();
            return;
        }

        if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
        {
            return;
        }
        else if (interactionLock.tipoActual == InteractionType.None)
        {
            interactionLock.Set(tipo);
        }

        if (Mouse.current == null) return;

        Camera cam = GetCam();
        if (cam == null) return;

        RecalcularPlano(cam);

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
        interactionLock.Limpiar();
    }

    void OnMouseDrag()
    {
        if (!playing) return;
        if (Mouse.current == null) return;

        if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
        {
            return;
        }

        Camera cam = GetCam();
        if (cam == null) return;

        RecalcularPlano(cam);

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

            if (!bloqueada && encajada && keySocket != null)
            {
                float yRel = (transform.position - keySocket.position).y;
                if (Mathf.Abs(yRel - bloqueoLocalOffset.y) <= yTolerancia)
                {
                    BloquearYLanzarEventos();
                }
            }
        }
    }

    private void BloquearYLanzarEventos()
    {
        bloqueada = true;

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
        abrirCandadoLlave.salir();
        controlMaquina.SetRojo();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entra");
        if (other.CompareTag("KeySocket"))
        {
            encajada = true;

            zEncajada = transform.position.z;
            xEncajada = transform.position.x;

            yEntrada = transform.position.y;

            if (keySocket == null)
                keySocket = other.transform;

            bloqueoLocalOffset = transform.position - keySocket.position;
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