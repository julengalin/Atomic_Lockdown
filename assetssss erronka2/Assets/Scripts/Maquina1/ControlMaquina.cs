using UnityEngine;

public class ControlMaquina : MonoBehaviour
{
    public GestionValvula valvulaRoja;
    public GestionValvula valvulaAzul;

    public AbrirPuerta abrirPuerta;

    bool yaSeAbrio = false;

    public GestionLuz luzAzul;
    public GestionLuz luzRoja;

    public bool candadoRAbierto = false;
    public bool candadoAAbierto = false;

    private void Update()
    {
        if (yaSeAbrio) return;
        if (valvulaRoja == null || valvulaAzul == null) return;

        bool rojoVerde = valvulaRoja.esVerde();
        bool azulVerde = valvulaAzul.esVerde();

        if (rojoVerde && azulVerde)
        {
            yaSeAbrio = true;

            valvulaRoja.Bloquear();
            valvulaAzul.Bloquear();

            if (abrirPuerta != null)
                abrirPuerta.Abrir();

            luzAzul.setAbierto();
            luzRoja.setAbierto();
        }
    }

    public void SetRojo()
    {
        Debug.Log("entraRojo");
        candadoRAbierto = true;
    }

    public void SetAzul()
    {
        Debug.Log("entraAzul");
        candadoAAbierto = true;
    }

    public void avisoSalida()
    {
        valvulaAzul.JugadorHaSalido();
        valvulaRoja.JugadorHaSalido();
    }
}
