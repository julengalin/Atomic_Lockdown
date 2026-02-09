using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbrirCandadoLlave : MonoBehaviour
{
    public GestionLlave gestionLlave;
    public RecogerLlave recogerLlave;

    [SerializeField] public Camera cam;

    public GameObject key;

    public bool playMode = false;

    public Vector3 candadoRotCorrecta = new Vector3(270f, 0f, 90f);

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoLlave;

    private void OnMouseDown()
    {
        metodoClick();
    }

    public void metodoClick()
    {
        Debug.Log("Click");
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

        gestionLlave.abrirLlave();
    }
}
