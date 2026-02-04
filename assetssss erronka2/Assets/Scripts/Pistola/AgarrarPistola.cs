using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AgarrarPistola : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRGrabInteractable grab;

    [Header("Interactores que agarran la pistola")]
    public XRBaseInteractor leftInteractor;
    public XRBaseInteractor rightInteractor;

    [Header("SOLO el modelo visual del mando")]
    public GameObject leftControllerVisual;
    public GameObject rightControllerVisual;

    [Header("Bloquear la pistola en la mano")]
    public bool lockInHand = true;

    private enum Side { None, Left, Right }
    private Side currentSide = Side.None;

    // 👇 Mano “dueña” (la primera que la agarró)
    private IXRSelectInteractor ownerInteractor = null;
    private Side ownerSide = Side.None;

    void Awake()
    {
        if (!grab) grab = GetComponent<XRGrabInteractable>();

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // Si todavía no hay dueño, el primero que la coge se queda como dueño
        if (ownerInteractor == null)
        {
            ownerInteractor = args.interactorObject;

            if (args.interactorObject == leftInteractor) ownerSide = Side.Left;
            else if (args.interactorObject == rightInteractor) ownerSide = Side.Right;
            else ownerSide = Side.None;

            currentSide = ownerSide;

            // Oculta SOLO el mando del dueño
            if (ownerSide == Side.Left && leftControllerVisual) leftControllerVisual.SetActive(false);
            if (ownerSide == Side.Right && rightControllerVisual) rightControllerVisual.SetActive(false);

            return;
        }

        // Si alguien que NO es el dueño intenta agarrarla -> no permitimos cambio de mano
        if (args.interactorObject != ownerInteractor && grab.interactionManager != null)
        {
            // Soltamos al "ladrón" y volvemos a agarrar con el dueño
            grab.interactionManager.SelectExit(args.interactorObject, grab);
            grab.interactionManager.SelectEnter(ownerInteractor, grab);
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (lockInHand && grab.interactionManager != null && ownerInteractor != null)
        {
            // Fuerza que la pistola NO se suelte nunca (siempre vuelve al dueño)
            grab.interactionManager.SelectEnter(ownerInteractor, grab);
            return;
        }

        // Solo si permites soltar (lockInHand = false)
        if (ownerSide == Side.Left && leftControllerVisual)
            leftControllerVisual.SetActive(true);

        if (ownerSide == Side.Right && rightControllerVisual)
            rightControllerVisual.SetActive(true);

        ownerInteractor = null;
        ownerSide = Side.None;
        currentSide = Side.None;
    }
}
