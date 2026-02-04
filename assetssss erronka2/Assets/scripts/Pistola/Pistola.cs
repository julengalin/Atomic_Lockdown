using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pistola : MonoBehaviour
{
    [Header("Disparo")]
    public LineRenderer linePrefab;
    public Transform shootingPoint;
    public float maxLineDistance = 5f;
    public float lineShowTimer = 0.3f;
    public DisparoTipo tipoDisparo = DisparoTipo.Rojo;

    [Header("XR")]
    [SerializeField] private XRGrabInteractable grab;
    [SerializeField] private float fireCooldown = 0.15f;

    private float lastFireTime;
    private bool triggerHeld;

    void Awake()
    {
        if (!grab) grab = GetComponent<XRGrabInteractable>();

        // Detecta cuando se pulsa el "Activate" del interactable (por defecto: trigger)
        grab.activated.AddListener(OnActivated);
        grab.deactivated.AddListener(OnDeactivated);
    }

    void OnDestroy()
    {
        grab.activated.RemoveListener(OnActivated);
        grab.deactivated.RemoveListener(OnDeactivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        triggerHeld = true;
        TryFire();
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        triggerHeld = false;
    }

    void Update()
    {
        // si quieres disparo automático manteniendo trigger:
        if (triggerHeld) TryFire();
    }

    private void TryFire()
    {
        if (Time.time - lastFireTime < fireCooldown) return;
        lastFireTime = Time.time;
        Disparar();
    }

    void Disparar()
    {
        if (!linePrefab || !shootingPoint) return;

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, shootingPoint.position);

        Vector3 endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance;

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxLineDistance))
        {
            IRecibeDisparo r = hit.collider.GetComponentInParent<IRecibeDisparo>();
            if (r != null) r.RecibirDisparo(tipoDisparo);

            endPoint = hit.point;
        }

        line.SetPosition(1, endPoint);
        Destroy(line.gameObject, lineShowTimer);
    }
}
