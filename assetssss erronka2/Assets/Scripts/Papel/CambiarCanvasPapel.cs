using UnityEngine;
using UnityEngine.InputSystem;

public class CambiarCanvasPapel : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;

    [SerializeField] Camera cam;
    [SerializeField] Vector3 offset = new Vector3(0f, 0f, 0.6f);
    [SerializeField] bool faceCamera = true;

    [SerializeField] string sortingLayerName = "VR_UI";
    [SerializeField] int sortingOrder = 100;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.Papel;

    float lastToggleTime;
    [SerializeField] float toggleCooldown = 0.15f;

    void Update()
    {
        if (canvasObject != null && canvasObject.activeSelf)
        {
            ActualizarPosicionCanvas();
        }
    }

    public void ToggleCanvas()
    {
        if (Time.time - lastToggleTime < toggleCooldown)
        {
            Debug.Log("ha pasao poco desde el hit");
            return;
        }

        lastToggleTime = Time.time;

        Debug.Log("entra");
        if (canvasObject == null) return;

        Debug.Log("existe canvas");
        bool estabaAbierto = canvasObject.activeSelf;
        Debug.Log("Estaba abierto?" + estabaAbierto);

        if (estabaAbierto)
        {
            canvasObject.SetActive(false);
            if (interactionLock != null) interactionLock.Limpiar();
            return;
        }

        Debug.Log("Estaba cerrado");

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

        Debug.Log("Hay interactionlock y ahora el tipo es:" + interactionLock.tipoActual);

        if (cam == null) cam = Camera.main;

        Canvas c = canvasObject.GetComponent<Canvas>();
        if (c != null)
        {
            c.overrideSorting = true;
            c.sortingLayerName = sortingLayerName;
            c.sortingOrder = sortingOrder;
        }

        ActualizarPosicionCanvas();

        canvasObject.SetActive(true);
        Debug.Log("Activo");
        Debug.Log(canvasObject.activeSelf);
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