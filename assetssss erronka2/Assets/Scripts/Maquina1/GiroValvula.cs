using UnityEngine;

public class GiroValvulaVisual : MonoBehaviour
{
    public GestionValvula gestionValvula;

    public InteractionLock interactionLock;
    public InteractionType tipo;

    public float minAngulo = 0f;
    public float maxAngulo = 360f;

    public int pasos = 10;

    [SerializeField] Transform ejeReferencia;
    [SerializeField] Vector3 ejeLocal = Vector3.up;

    [SerializeField] float gradosPorPulsacion = 5f;

    float anguloActual = 0f;

    Quaternion baseLocalRotation;

    private void Awake()
    {
        if (ejeReferencia == null) ejeReferencia = transform;
        baseLocalRotation = ejeReferencia.localRotation;

        if (gestionValvula != null)
        {
            anguloActual = ValorAAngulo(gestionValvula.valorActual);
            AplicarRotacion();

            // ?? Actualizamos la luz ya en Awake
            gestionValvula.SetValorActual(AnguloAValor(anguloActual));
        }
    }

    private void Start()
    {
        if (gestionValvula != null)
        {
            anguloActual = ValorAAngulo(gestionValvula.valorActual);
            AplicarRotacion();
            gestionValvula.SetValorActual(AnguloAValor(anguloActual));
        }
    }

    public void metodoSelect()
    {
        if (interactionLock != null)
        {
            if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
                return;

            if (interactionLock.tipoActual == InteractionType.None)
                interactionLock.Set(tipo);
        }

        if (gestionValvula == null) return;
        if (!gestionValvula.playMode) return;

        baseLocalRotation = ejeReferencia.localRotation;
    }

    public void metodoUnselect()
    {
        gestionValvula.Salir();
    }

    public void ForzarIzquierda()
    {
        Debug.Log("Entra izquierda");
        ForzarPaso(-1f);
    }

    public void ForzarDerecha()
    {
        Debug.Log("Entra derecha");
        ForzarPaso(1f);
    }

    void ForzarPaso(float dir)
    {
        if (gestionValvula == null) return;
        if (gestionValvula.EstaBloqueada()) return;

        Debug.Log("Pasa los returns");

        anguloActual = Mathf.Clamp(anguloActual + dir * gradosPorPulsacion, minAngulo, maxAngulo);

        AplicarRotacion();

        gestionValvula.SetValorActual(AnguloAValor(anguloActual));
    }

    void AplicarRotacion()
    {
        ejeReferencia.localRotation = baseLocalRotation * Quaternion.AngleAxis(anguloActual, ejeLocal);
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
}