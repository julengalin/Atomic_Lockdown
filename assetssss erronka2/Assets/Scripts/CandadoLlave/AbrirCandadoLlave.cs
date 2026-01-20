using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbrirCandadoLlave : MonoBehaviour
{
    public GestionLlave gestionLlave;
    public RecogerLlave recogerLlave;
    [SerializeField] public Camera cam;
    public Button button;

    public GameObject key;

    public bool playMode = false;
    Vector3 posicionInicial;
    Quaternion rotacionInicial;

    Vector3 candadoOffset = new Vector3(1.0000f, 0.65f, 7.0672f);
    Vector3 llaveOffset = new Vector3(-5.477f, -1f, 6.899f);

    public Vector3 candadoRotCorrecta = new Vector3(270f, 0f, 90f);
    Quaternion candadoRotOffset;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoLlave;

    private void Start()
    {
        posicionInicial = gameObject.transform.position;
        rotacionInicial = gameObject.transform.rotation;

        if (cam == null) cam = Camera.main;
        if (cam != null)
            candadoRotOffset = Quaternion.Inverse(cam.transform.rotation) * Quaternion.Euler(candadoRotCorrecta);
    }

    private void OnMouseDown()
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

        if (!playMode) gestionLlave.abrirLlave();
    }

    Vector3 CamOffset(Camera c, Vector3 offset)
    {
        return c.transform.position
             + c.transform.right * offset.x
             + c.transform.up * offset.y
             + c.transform.forward * offset.z;
    }

    public void ampliar()
    {
        playMode = true;
        button.gameObject.SetActive(true);

        Debug.Log(playMode);

        if (cam == null) cam = Camera.main;

        transform.position = CamOffset(cam, candadoOffset);
        transform.rotation = cam.transform.rotation * candadoRotOffset;

        key.SetActive(true);
        key.transform.position = CamOffset(cam, llaveOffset);
        key.transform.rotation = Quaternion.Euler(270f, 0f, 90f);

        recogerLlave.inGame();
    }

    public void salir()
    {
        playMode = false;
        button.gameObject.SetActive(false);
        key.SetActive(false);

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        recogerLlave.exitGame();

        if (interactionLock != null)
            interactionLock.Limpiar();
    }
}
