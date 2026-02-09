using UnityEngine;

public class PalancaEmergencia : MonoBehaviour, IRecibeDisparo
{
    [Header("Puertas que deben entrar en emergencia")]
    [SerializeField] private DoorEyeLock[] puertas;

    [Header("Rotación de la palanca")]
    [SerializeField] private Transform palancaVisual;          // si es null, usa este transform
    [SerializeField] private Vector3 ejeLocal = Vector3.right; // eje local de giro
    [SerializeField] private float anguloEmergencia = 120f;    // ángulo objetivo
    [SerializeField] private float velocidad = 180f;           // grados/segundo

    [Header("Opcional: solo responde a este tipo")]
    [SerializeField] private bool filtrarPorTipo = false;
    [SerializeField] private DisparoTipo tipoRequerido = DisparoTipo.Rojo;

    private bool activada;
    private bool girando;
    private float anguloActual;

    void Awake()
    {
        if (palancaVisual == null) palancaVisual = transform;
        // Si la palanca ya tiene rotación inicial, puedes ajustar anguloActual aquí si lo necesitas.
        // Por simplicidad lo dejamos en 0 (posición "reposo").
    }

    void Update()
    {
        if (!girando) return;

        anguloActual = Mathf.MoveTowards(anguloActual, anguloEmergencia, velocidad * Time.deltaTime);
        palancaVisual.localRotation = Quaternion.AngleAxis(anguloActual, ejeLocal);

        if (Mathf.Abs(anguloActual - anguloEmergencia) < 0.01f)
            girando = false;
    }

    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (activada) return;

        if (filtrarPorTipo && tipo != tipoRequerido)
            return;

        activada = true;

        // Flag global (por si lo usas en otros sitios)
        ActivarEmergencia.EmergenciaActiva = true;

        // Activa emergencia en cada DoorEyeLock (esto es lo importante para ojos/bloqueo)
        if (puertas != null)
        {
            for (int i = 0; i < puertas.Length; i++)
            {
                if (puertas[i] != null)
                    puertas[i].ActivarEmergencia();
            }
        }

        // Gira la palanca
        girando = true;
    }
}
