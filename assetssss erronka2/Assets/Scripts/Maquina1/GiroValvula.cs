using UnityEngine;

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

    float anguloBase;

    float screenWidth;
    Vector3 pressPoint;
    Quaternion startRotation;

    float velocidadGiro = 0.35f;

    private void Start()
    {
        screenWidth = Screen.width;
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

        if (gestionValvula == null) return;
        if (!gestionValvula.playMode) return;

        pressPoint = Input.mousePosition;
        anguloBase = anguloActual;

        arrastrando = true;
    }

    private void OnMouseUp()
    {
        arrastrando = false;
    }

    private void Update()
    {
        if (!arrastrando) return;
        if (gestionValvula.EstaBloqueada()) return;

        if (interactionLock != null && interactionLock.tipoActual != tipo) return;

        if (Input.GetMouseButtonDown(0))
        {
            pressPoint = Input.mousePosition;
            startRotation = transform.rotation;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = (Input.mousePosition - pressPoint).x;

            float deltaAngulo = (deltaX / screenWidth) * 360f * velocidadGiro;
            float anguloDeseado = anguloBase + deltaAngulo;
            anguloActual = Mathf.Clamp(anguloDeseado, minAngulo, maxAngulo);

            transform.rotation = startRotation * Quaternion.Euler(0f, anguloActual, 0f);

            if (gestionValvula != null)
                gestionValvula.SetValorActual(AnguloAValor(anguloActual));
        }
    }

    int AnguloAValor(float angulo)
    {
        if (pasos <= 1) return 0;

        float t = Mathf.InverseLerp(minAngulo, maxAngulo, angulo);
        int v = Mathf.RoundToInt(t * (pasos - 1));
        return Mathf.Clamp(v, 0, pasos - 1);
    }

    public void ReiniciarVisual(float anguloInicial)
    {
        anguloActual = Mathf.Clamp(anguloInicial, minAngulo, maxAngulo);
        transform.localRotation = Quaternion.Euler(0f, anguloActual, 0f);
    }
}
