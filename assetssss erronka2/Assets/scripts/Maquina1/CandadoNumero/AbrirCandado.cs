using UnityEngine;

public class AbrirCandado : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;
    public bool playMode = false;
    [SerializeField] Camera cam;
    Vector3 posicionInicial;
    Quaternion rotacionInicial;
    Vector3 escalaInicial;

    [SerializeField] Vector3 candadoOffset = new Vector3(1.0000f, 0.65f, 5.5f);

    public Vector3 candadoRotCorrecta = new Vector3(0f, 0f, 0f);
    Quaternion candadoRotOffset;

    [SerializeField] Transform CandadoNumeros;

    Quaternion camRotInicial;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoNumerico;

    public float escala = 3f;

    private void Start()
    {
        posicionInicial = gameObject.transform.position;
        rotacionInicial = gameObject.transform.rotation;
        escalaInicial = gameObject.transform.localScale;

        candadoRotOffset = Quaternion.Euler(candadoRotCorrecta);

        if (CandadoNumeros == null)
            CandadoNumeros = transform;
    }

    void Update()
    {
        if (!playMode) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        transform.position = CamOffset(cam, candadoOffset);

        Vector3 dir = CandadoNumeros.position - cam.transform.position;
        if (dir.sqrMagnitude > 0.000001f)
            CandadoNumeros.rotation = Quaternion.LookRotation(dir, Vector3.up) * candadoRotOffset;
    }

    Vector3 CamOffset(Camera c, Vector3 offset)
    {
        return c.transform.position
             + c.transform.right * offset.x
             + c.transform.up * offset.y
             + c.transform.forward * offset.z;
    }

    void OnMouseDown()
    {
        metodoClick();
    }

    public void metodoClick()
    {
        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
            {
                return;
            }
            else if (interactionLock.tipoActual == InteractionType.None)
            {
                interactionLock.Set(tipo);
            }
        }

        if (!playMode) ToggleState();
    }

    public void ToggleState()
    {
        playMode = !playMode;
        Debug.Log(playMode);

        GetComponent<Collider>().enabled = !playMode;

        if (cam == null) cam = Camera.main;

        if (playMode)
        {
            transform.localScale = escalaInicial * 3f;

            if (cam != null)
            {
                camRotInicial = cam.transform.rotation;

                transform.position = CamOffset(cam, candadoOffset);

                Vector3 dir = CandadoNumeros.position - cam.transform.position;
                if (dir.sqrMagnitude > 0.000001f)
                    CandadoNumeros.rotation = Quaternion.LookRotation(dir, Vector3.up) * candadoRotOffset;
            }
        }
        else
        {
            transform.position = posicionInicial;
            transform.rotation = rotacionInicial;
            transform.localScale = escalaInicial;
        }

        if (canvasObject == null) return;

        canvasObject.SetActive(!canvasObject.activeSelf);

        if (!playMode && interactionLock != null)
            interactionLock.Limpiar();
    }
}
