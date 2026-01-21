using UnityEngine;
using UnityEngine.InputSystem;

public class GirarPalanca : MonoBehaviour
{
    [SerializeField] float minAngulo = 0f;
    [SerializeField] float maxAngulo = 120f;
    [SerializeField] float velocidad = 0.1f;

    [SerializeField] Vector3 ejeLocal = Vector3.right;

    [SerializeField] Light luz;
    [SerializeField] float intensidadBase = 1f;
    [SerializeField] float gradosPorPaso = 15f;

    [SerializeField] GestionLiquido gestionLiquido;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.Palanca;

    float anguloActual;
    float lastMouseY;
    bool dragging;
    bool bloqueada;

    void Start()
    {
        SetAngulo(anguloActual);
        ActualizarLuz();
        if (gestionLiquido != null) gestionLiquido.cambiarFill(anguloActual);
    }

    void OnMouseDown()
    {
        if (bloqueada) return;

        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo) return;
            if (interactionLock.tipoActual == InteractionType.None) interactionLock.Set(tipo);
        }

        if (Mouse.current == null) return;

        lastMouseY = Mouse.current.position.ReadValue().y;
        dragging = true;
    }

    void OnMouseDrag()
    {
        if (bloqueada) return;
        if (!dragging) return;
        if (Mouse.current == null) return;

        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo) return;
        }

        float y = Mouse.current.position.ReadValue().y;
        float dy = y - lastMouseY;
        lastMouseY = y;

        float delta = -dy * velocidad;
        SetAngulo(anguloActual + delta);
        ActualizarLuz();

        if (gestionLiquido != null) gestionLiquido.cambiarFill(anguloActual);
    }

    void OnMouseUp()
    {
        dragging = false;
        bloqueada = true;

        if (interactionLock != null && interactionLock.tipoActual == tipo)
            interactionLock.Limpiar();
    }

    void SetAngulo(float angulo)
    {
        anguloActual = Mathf.Clamp(angulo, minAngulo, maxAngulo);
        transform.localRotation = Quaternion.AngleAxis(anguloActual, ejeLocal);
    }

    void ActualizarLuz()
    {
        if (luz == null) return;

        int pasos = Mathf.FloorToInt(anguloActual / gradosPorPaso);
        luz.intensity = intensidadBase + pasos;
    }

    public void ResetearPalanca()
    {
        bloqueada = false;
        dragging = false;

        if (interactionLock != null && interactionLock.tipoActual == tipo)
            interactionLock.Limpiar();

        SetAngulo(minAngulo);
        ActualizarLuz();

        if (gestionLiquido != null) gestionLiquido.cambiarFill(anguloActual);
    }

    public float GetAngulo()
    {
        return anguloActual;
    }

    public bool EstaMaximo()
    {
        return Mathf.Abs(anguloActual - maxAngulo) < 0.01f;
    }

    public void ForzarAngulo(float angulo)
    {
        SetAngulo(angulo);
        ActualizarLuz();
        if (gestionLiquido != null) gestionLiquido.cambiarFill(anguloActual);
    }

    public void Desbloquear()
    {
        bloqueada = false;
    }

    public void Bloquear()
    {
        bloqueada = true;
        dragging = false;

        if (interactionLock != null && interactionLock.tipoActual == tipo)
            interactionLock.Limpiar();
    }
}
