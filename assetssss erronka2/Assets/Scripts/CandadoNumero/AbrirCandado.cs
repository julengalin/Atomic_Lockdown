using UnityEngine;
using UnityEngine.InputSystem;

public class AbrirCandado : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;
    public bool playMode = false;
    [SerializeField] Camera cam;
    Vector3 posicionInicial;
    Quaternion rotacionInicial;

    Vector3 candadoOffset = new Vector3(1.0000f, 0.65f, 5.5f);

    public Vector3 candadoRotCorrecta = new Vector3(0f, 0f, 90f);
    Quaternion candadoRotOffset;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoNumerico;

    private void Start()
    {
        posicionInicial = gameObject.transform.position;
        rotacionInicial = gameObject.transform.rotation;

        if (cam == null) cam = Camera.main;
        if (cam != null)
            candadoRotOffset = Quaternion.Inverse(cam.transform.rotation) * Quaternion.Euler(candadoRotCorrecta);
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
            transform.position = CamOffset(cam, candadoOffset);
            transform.rotation = cam.transform.rotation * candadoRotOffset;
        }
        else
        {
            transform.position = posicionInicial;
            transform.rotation = rotacionInicial;
        }

        if (canvasObject == null) return;

        canvasObject.SetActive(!canvasObject.activeSelf);

        if (!playMode && interactionLock != null)
            interactionLock.Limpiar();
    }
}
