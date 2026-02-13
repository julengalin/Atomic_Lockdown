using UnityEngine;

public class AbrirCandado : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;
    public bool playMode = false;
    [SerializeField] Camera cam;

    Vector3 posicionInicial;
    Quaternion rotacionInicial;
    Vector3 escalaInicial;

    [SerializeField] Transform abridor;

    Vector3 abridorPosInicial;
    Quaternion abridorRotInicial;
    Vector3 abridorEscalaInicial;

    Vector3 abridorOffsetLocal;

    [SerializeField] Vector3 candadoOffset = new Vector3(1.0f, 0.65f, 5.5f);

    public Vector3 candadoRotCorrecta = Vector3.zero;
    Quaternion candadoRotOffset;

    [SerializeField] Vector3 normalLocalCandado = new Vector3(0, 0, -1);
    [SerializeField] Vector3 upLocalCandado = new Vector3(0, 1, 0);

    [SerializeField] Vector3 normalLocalAbridor = new Vector3(0, 1, 0);
    [SerializeField] Vector3 upLocalAbridor = new Vector3(0, 0, 1);

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoNumerico;

    public float escala = 3f;

    [SerializeField] Vector3 offset = new Vector3(0f, 0f, 0.6f);
    [SerializeField] bool faceCamera = true;

    [SerializeField] string sortingLayerName = "VR_UI";
    [SerializeField] int sortingOrder = 100;

    public bool abierto = false;

    private void Start()
    {
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        escalaInicial = transform.localScale;

        if (abridor != null)
        {
            abridorPosInicial = abridor.position;
            abridorRotInicial = abridor.rotation;
            abridorEscalaInicial = abridor.localScale;

            abridorOffsetLocal = transform.InverseTransformPoint(abridor.position);
        }

        candadoRotOffset = Quaternion.Euler(candadoRotCorrecta);
    }

    void Update()
    {
        if (!playMode) return;
        if (abierto) return;

        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Vector3 basePos = CamOffset(cam, candadoOffset);
        transform.position = basePos;

        Vector3 toCam = cam.transform.position - transform.position;

        if (toCam.sqrMagnitude > 0.000001f)
        {
            Quaternion rotCandado = CalcularRotacion(toCam, normalLocalCandado, upLocalCandado) * candadoRotOffset;
            transform.rotation = rotCandado;

            if (abridor != null)
            {
                abridor.position = transform.TransformPoint(abridorOffsetLocal);
                abridor.rotation = CalcularRotacion(toCam, normalLocalAbridor, upLocalAbridor);
            }
        }
        else
        {
            if (abridor != null)
                abridor.position = transform.TransformPoint(abridorOffsetLocal);
        }

        if (canvasObject != null && canvasObject.activeSelf)
            ActualizarPosicionCanvas();
    }

    Quaternion CalcularRotacion(Vector3 toCamWorld, Vector3 normalLocal, Vector3 upLocal)
    {
        Vector3 desiredNormalWorld = toCamWorld.normalized;
        Quaternion targetFrame = Quaternion.LookRotation(desiredNormalWorld, Vector3.up);

        Vector3 nL = normalLocal.normalized;
        Vector3 uL = upLocal.normalized;

        Vector3 rL = Vector3.Cross(uL, nL).normalized;
        uL = Vector3.Cross(nL, rL).normalized;

        Quaternion localFrame = Quaternion.LookRotation(nL, uL);

        return targetFrame * Quaternion.Inverse(localFrame);
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
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        if (!playMode) ToggleState();
    }

    public void ToggleState()
    {
        playMode = !playMode;

        GetComponent<Collider>().enabled = !playMode;

        if (cam == null) cam = Camera.main;

        if (playMode)
        {
            abierto = false;

            transform.localScale = escalaInicial * escala;

            if (abridor != null)
                abridor.localScale = abridorEscalaInicial * escala;
        }
        else
        {
            transform.position = posicionInicial;
            transform.rotation = rotacionInicial;
            transform.localScale = escalaInicial;

            if (abridor != null)
            {
                abridor.position = abridorPosInicial;
                abridor.rotation = abridorRotInicial;
                abridor.localScale = abridorEscalaInicial;
            }
        }

        if (canvasObject == null) return;

        bool newState = !canvasObject.activeSelf;
        canvasObject.SetActive(newState);

        if (newState)
        {
            Canvas c = canvasObject.GetComponent<Canvas>();
            if (c != null)
            {
                c.overrideSorting = true;
                c.sortingLayerName = sortingLayerName;
                c.sortingOrder = sortingOrder;
            }

            ActualizarPosicionCanvas();
        }

        if (!playMode && interactionLock != null)
            interactionLock.Limpiar();
    }

    public void MarcarAbierto()
    {
        abierto = true;
    }

    void ActualizarPosicionCanvas()
    {
        if (cam == null || canvasObject == null) return;

        Transform t = canvasObject.transform;

        t.position = cam.transform.position
                   + cam.transform.right * offset.x
                   + cam.transform.up * offset.y
                   + cam.transform.forward * offset.z;

        if (faceCamera)
        {
            Vector3 fwd = t.position - cam.transform.position;
            if (fwd.sqrMagnitude > 0.000001f)
                t.rotation = Quaternion.LookRotation(fwd, Vector3.up);
        }
    }
}