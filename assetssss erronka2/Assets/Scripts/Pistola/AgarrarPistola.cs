using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AgarrarPistola : MonoBehaviour
{
    [Header("XR")]
    [SerializeField] private XRGrabInteractable grab;

    [Header("Interactores (solo para saber qué mano oculta el visual)")]
    public XRBaseInteractor leftInteractor;
    public XRBaseInteractor rightInteractor;

    [Header("SOLO el modelo visual del mando (solo local)")]
    public GameObject leftControllerVisual;
    public GameObject rightControllerVisual;

    [Header("Bloquear la pistola en la mano")]
    public bool lockInHand = true;

    private enum Side { None, Left, Right }
    private Side ownerSide = Side.None;

    private IXRSelectInteractor localHeldInteractor = null;

    private PistolaNetwork net;
    private NetworkObject netObj;

    private Coroutine pendingRegrab;
    private float lastOwnershipRequestTime;

    void Awake()
    {
        if (!grab) grab = GetComponent<XRGrabInteractable>();
        net = GetComponent<PistolaNetwork>();
        netObj = GetComponent<NetworkObject>();

        // Pedimos ownership lo antes posible (hover)
        grab.hoverEntered.AddListener(OnHoverEntered);

        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        if (!grab) return;
        grab.hoverEntered.RemoveListener(OnHoverEntered);
        grab.selectEntered.RemoveListener(OnGrab);
        grab.selectExited.RemoveListener(OnRelease);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (net == null || netObj == null) return;

        // Anti-spam
        if (Time.time - lastOwnershipRequestTime < 0.25f) return;
        lastOwnershipRequestTime = Time.time;

        // Si ya soy owner, no hace falta
        if (netObj.IsOwner) return;

        // ✅ CUALQUIERA que haga hover en SU cliente pedirá ownership
        net.RequestGrab();
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (net == null || netObj == null) return;

        // Si aún no soy owner cuando se intenta agarrar, suelto y reintento cuando llegue el ownership
        if (!netObj.IsOwner)
        {
            // Asegura ownership
            net.RequestGrab();

            if (grab.interactionManager != null)
                grab.interactionManager.SelectExit(args.interactorObject, grab);

            if (pendingRegrab != null) StopCoroutine(pendingRegrab);
            pendingRegrab = StartCoroutine(WaitOwnershipAndRegrab(args.interactorObject));
            return;
        }

        // Ya soy owner: guardamos el interactor para lock local
        if (localHeldInteractor == null)
            localHeldInteractor = args.interactorObject;

        // Detectar mano (solo para ocultar visual local)
        var xrInteractor = args.interactorObject as XRBaseInteractor;
        if (xrInteractor != null && xrInteractor == leftInteractor) ownerSide = Side.Left;
        else if (xrInteractor != null && xrInteractor == rightInteractor) ownerSide = Side.Right;
        else ownerSide = Side.None;

        // Ocultar mando SOLO en este cliente
        if (ownerSide == Side.Left && leftControllerVisual) leftControllerVisual.SetActive(false);
        if (ownerSide == Side.Right && rightControllerVisual) rightControllerVisual.SetActive(false);
    }

    IEnumerator WaitOwnershipAndRegrab(IXRSelectInteractor interactor)
    {
        float timeout = 2f;
        while (timeout > 0f && netObj != null && !netObj.IsOwner)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        if (netObj == null || !netObj.IsOwner) yield break;
        if (grab == null || grab.interactionManager == null) yield break;

        grab.interactionManager.SelectEnter(interactor, grab);
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // Lock local (no suelta)
        if (lockInHand && grab.interactionManager != null && localHeldInteractor != null)
        {
            grab.interactionManager.SelectEnter(localHeldInteractor, grab);
            return;
        }

        // Si permites soltar:
        if (!lockInHand && net != null)
            net.RequestRelease();

        // Mostrar mando local
        if (ownerSide == Side.Left && leftControllerVisual) leftControllerVisual.SetActive(true);
        if (ownerSide == Side.Right && rightControllerVisual) rightControllerVisual.SetActive(true);

        localHeldInteractor = null;
        ownerSide = Side.None;

        if (pendingRegrab != null)
        {
            StopCoroutine(pendingRegrab);
            pendingRegrab = null;
        }
    }
}
