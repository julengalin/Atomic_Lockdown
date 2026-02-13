using UnityEngine;

public class GestionValvula : MonoBehaviour
{
    public InteractionLock interactionLock;
    public InteractionType tipo;

    public bool playMode = false;

    public int valorCorrecto;
    public int valorInicial;
    public int valorActual;

    public GameObject candado;

    public GestionLuz gestionLuz;

    bool bloqueada = false;

    public ControlMaquina1 controlMaquina;

    int lastDiff = int.MinValue;

    public GiroValvulaVisual visual;

    private void Start()
    {
        valorActual = valorInicial;

        if (gestionLuz != null)
            gestionLuz.SetDiff(valorActual - valorCorrecto);
    }

    private void Update()
    {
        int diff = valorActual - valorCorrecto;
        if (diff != lastDiff)
        {
            lastDiff = diff;
            if (gestionLuz != null)
                gestionLuz.SetDiff(diff);
        }
    }

    private void OnMouseDown()
    {
        metodoClick();
    }

    public void metodoClick()
    {
        Debug.Log("Llega");
        if (tipo == InteractionType.ValvulaAzul)
        {
            Debug.Log("entra mouse azul");
            if (controlMaquina.candadoAAbierto != true)
            {
                Debug.Log("azul cerrado");
                return;
            }
        }
        else if (tipo == InteractionType.ValvulaRoja)
        {
            Debug.Log("entra mouse azul");
            if (controlMaquina.candadoRAbierto != true)
            {
                Debug.Log("rojo cerrado");
                return;
            }
        }
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

        Debug.Log("Pasa interacciones va a entrar");
        if (!playMode) Entrar();
    }

    public void Entrar()
    {
        Debug.Log("Ha entrado");
        playMode = true;

        if (candado != null) candado.SetActive(false);
        Debug.Log("Vamos al metodo select");
        visual.metodoSelect();
    }

    public void Salir()
    {
        playMode = false;

        if (candado != null) candado.SetActive(true);

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

    public void aviso()
    {
        controlMaquina.avisoSalida();
    }

    public void SetValorActual(int nuevoValor)
    {
        Debug.Log("Entramos en setvaloractual");
        valorActual = nuevoValor;

        Debug.Log("nuevoValor = " + nuevoValor);
        Debug.Log("valorActual = " + valorActual);
        if (gestionLuz != null)
        {
            Debug.Log("Gestion luz no está vacio");
            gestionLuz.SetDiff(valorActual - valorCorrecto);
        }
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
