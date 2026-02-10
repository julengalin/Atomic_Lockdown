using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Pistola : NetworkBehaviour
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

    private float lastFireTimeLocal;
    private bool triggerHeld;

    void Awake()
    {
        if (!grab) grab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (!grab) return;
        grab.activated.AddListener(OnActivated);
        grab.deactivated.AddListener(OnDeactivated);
    }

    void OnDisable()
    {
        if (!grab) return;
        grab.activated.RemoveListener(OnActivated);
        grab.deactivated.RemoveListener(OnDeactivated);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        if (!IsOwner) return;
        triggerHeld = true;
        TryFireLocal();
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        if (!IsOwner) return;
        triggerHeld = false;
    }

    void Update()
    {
        if (!IsOwner) return;
        if (triggerHeld) TryFireLocal();
    }

    private void TryFireLocal()
    {
        if (Time.time - lastFireTimeLocal < fireCooldown) return;
        lastFireTimeLocal = Time.time;

        if (!shootingPoint) return;

        FireRpc(shootingPoint.position, shootingPoint.forward, tipoDisparo);
    }

    // ✅ NUEVO atributo en lugar de [ServerRpc]
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void FireRpc(Vector3 origin, Vector3 direction, DisparoTipo tipo)
    {
        Vector3 endPoint = origin + direction * maxLineDistance;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxLineDistance))
        {
            endPoint = hit.point;

            var r = hit.collider.GetComponentInParent<IRecibeDisparo>();
            if (r != null) r.RecibirDisparo(tipo);
        }

        FireFxRpc(origin, endPoint);
    }

    [Rpc(SendTo.Everyone)]
    private void FireFxRpc(Vector3 start, Vector3 end)
    {
        if (!linePrefab) return;

        var line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        Destroy(line.gameObject, lineShowTimer);
    }
}
