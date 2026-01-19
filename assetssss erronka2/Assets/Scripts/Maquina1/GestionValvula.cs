using UnityEngine;

public class GestionValvula : MonoBehaviour
{
    public Camera cam;
    public GameObject botonSalir;

    public InteractionLock interactionLock;
    public InteractionType tipo;

    public bool playMode = false;

    public Vector3 camPlayOffset = new Vector3(-9.184861f, 7.47298f, -16.25878f);
    public Vector3 camPlayRotation = new Vector3(-150f, 29.463f, 0f);

    public int valorCorrecto;
    public int valorInicial;
    public int valorActual;

    public GameObject candado;

    [SerializeField] Vector3 camPosInicial;
    [SerializeField] Quaternion camRotInicial;

    static bool camaraBloqueada = false;
    static Vector3 camPosGlobal;
    static Quaternion camRotGlobal;

    public Collider col;
    public Collider colHijo;

    public GestionLuz gestionLuz;

    bool bloqueada = false;

    private void Start()
    {
        valorActual = valorInicial;

        col = GetComponent<Collider>();

        if (botonSalir != null) botonSalir.SetActive(false);

        if (gestionLuz != null)
            gestionLuz.SetDiff(valorActual - valorCorrecto);
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

        if (!playMode) Entrar();
    }

    public void Entrar()
    {
        if (cam == null) return;

        playMode = true;

        if (!camaraBloqueada)
        {
            camPosGlobal = cam.transform.position;
            camRotGlobal = cam.transform.rotation;
            camaraBloqueada = true;
        }

        if (candado != null) candado.SetActive(false);

        if (botonSalir != null) botonSalir.SetActive(true);

        if (col != null) col.enabled = false;
        if (colHijo != null) colHijo.enabled = true;

        cam.transform.position = transform.position + camPlayOffset;
        cam.transform.rotation = Quaternion.Euler(camPlayRotation);
    }

    public void Salir()
    {
        if (cam == null) return;

        playMode = false;

        if (botonSalir != null) botonSalir.SetActive(false);

        cam.transform.position = camPosGlobal;
        cam.transform.rotation = camRotGlobal;

        camaraBloqueada = false;

        if (candado != null) candado.SetActive(true);

        if (colHijo != null) colHijo.enabled = false;
        if (col != null) col.enabled = true;

        if (interactionLock != null)
        {
            interactionLock.Limpiar();
        }
    }

    public void Reiniciar()
    {
        valorActual = valorInicial;

        if (gestionLuz != null)
            gestionLuz.SetDiff(valorActual - valorCorrecto);
    }

    public void JugadorHaSalido()
    {
        Reiniciar();
        Salir();
    }

    public void SetValorActual(int nuevoValor)
    {
        valorActual = nuevoValor;

        if (gestionLuz != null)
            gestionLuz.SetDiff(valorActual - valorCorrecto);
    }

    public bool esVerde()
    {
        if (valorActual == valorCorrecto) return true;
        else return false;
    }

    public void Bloquear()
    {
        bloqueada = true;
    }

    public bool EstaBloqueada()
    {
        return bloqueada;
    }
}
