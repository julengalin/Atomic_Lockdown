using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class GiroValvulaVisual : MonoBehaviour
{
    public GestionValvula gestionValvula;

    public InteractionLock interactionLock;
    public InteractionType tipo;

    public float minAngulo = 0f;
    public float maxAngulo = 360f;

    public int pasos = 10;

    bool arrastrando = false;

    float anguloActual = 0f;

    float stepAcc = 0f;

    Quaternion baseLocalRotation;

    [SerializeField] float deadzone = 0.15f;

    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        baseLocalRotation = transform.localRotation;
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (gestionValvula != null)
        {
            anguloActual = ValorAAngulo(gestionValvula.valorActual);
            AplicarRotacion();
        }
    }

    public void metodoSelect()
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

        Debug.Log("Vuelve a pasar interacciones");

        if (gestionValvula == null) return;
        if (!gestionValvula.playMode) return;

        arrastrando = true;
        Debug.Log("Arrastrando = " + arrastrando.ToString());
    }

    public void metodoUnselect()
    {
        if (arrastrando && gestionValvula != null)
            gestionValvula.JugadorHaSalido();

        arrastrando = false;
    }

    private void OnMouseDown()
    {
        metodoSelect();
    }

    private void OnMouseUp()
    {
        metodoUnselect();
    }

    private void Update()
    {
        if (!arrastrando) return;
        Debug.Log("Está arrastrando");

        if (gestionValvula.EstaBloqueada()) return;
        Debug.Log("No esta bloqueada");

        if (interactionLock != null && interactionLock.tipoActual != tipo) return;
        Debug.Log("Interaction bien");

        float inputDirection = 0f;
        OVRInput.Controller controllerToCheck = OVRInput.Controller.Active;

        if (grabInteractable != null && grabInteractable.isSelected && grabInteractable.interactorsSelecting.Count > 0)
        {
            var interactor = grabInteractable.interactorsSelecting[0];

            string n = interactor.transform.name.ToLower();

            if (n.Contains("left") || n.Contains("izq"))
                controllerToCheck = OVRInput.Controller.LTouch;
            else if (n.Contains("right") || n.Contains("der"))
                controllerToCheck = OVRInput.Controller.RTouch;
            else
            {
                if (interactor.transform.root.name.ToLower().Contains("left") || interactor.transform.root.name.ToLower().Contains("izq"))
                    controllerToCheck = OVRInput.Controller.LTouch;
                else if (interactor.transform.root.name.ToLower().Contains("right") || interactor.transform.root.name.ToLower().Contains("der"))
                    controllerToCheck = OVRInput.Controller.RTouch;
            }

        }

        if (OVRInput.GetDown(OVRInput.Button.One, controllerToCheck))
        {
            inputDirection = 1f;
        }
        else if (OVRInput.GetDown(OVRInput.Button.Two, controllerToCheck))
        {
            inputDirection = -1f;
        }

        if (inputDirection == 0f)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) inputDirection = 1f;
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) inputDirection = -1f;
        }

        Debug.Log("Direccion Input: " + inputDirection.ToString());

        if (inputDirection == 0f) return;

        if (pasos <= 1) return;

        float anguloPorPaso = (maxAngulo - minAngulo) / (pasos - 1);

        stepAcc += 1f;

        if (stepAcc < 1f) return;

        int pasosAAvanzar = Mathf.FloorToInt(stepAcc);
        stepAcc -= pasosAAvanzar;

        float nuevoAngulo = anguloActual + inputDirection * anguloPorPaso * pasosAAvanzar;

        anguloActual = Mathf.Clamp(nuevoAngulo, minAngulo, maxAngulo);

        Debug.Log("anguloActual" + anguloActual.ToString());

        AplicarRotacion();

        if (gestionValvula != null)
            Debug.Log("Gestion valvula no es nulo");

        gestionValvula.SetValorActual(AnguloAValor(anguloActual));
    }

    void AplicarRotacion()
    {
        Debug.Log("Entra en aplicar rotación");
        Debug.Log(" baseLocalRotation * Quaternion.Euler(0f, anguloActual, 0f) = " + (baseLocalRotation * Quaternion.Euler(0f, anguloActual, 0f)).ToString());

        transform.localRotation = baseLocalRotation * Quaternion.Euler(0f, anguloActual, 0f);
        Debug.Log("local rotation = " + transform.localRotation.ToString());
    }

    int AnguloAValor(float angulo)
    {
        if (pasos <= 1) return 0;

        float t = Mathf.InverseLerp(minAngulo, maxAngulo, angulo);
        int v = Mathf.RoundToInt(t * (pasos - 1));
        return Mathf.Clamp(v, 0, pasos - 1);
    }

    float ValorAAngulo(int valor)
    {
        if (pasos <= 1) return minAngulo;

        int v = Mathf.Clamp(valor, 0, pasos - 1);
        float t = v / (float)(pasos - 1);
        return Mathf.Lerp(minAngulo, maxAngulo, t);
    }

    public void ReiniciarVisual(float anguloInicial)
    {
        anguloActual = Mathf.Clamp(anguloInicial, minAngulo, maxAngulo);
        AplicarRotacion();
    }
}
