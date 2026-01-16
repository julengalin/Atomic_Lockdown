using UnityEngine;

public class ControlMaquina : MonoBehaviour
{
    public GestionValvula valvulaRoja;
    public GestionValvula valvulaAzul;

    public AbrirPuerta abrirPuerta;

    bool yaSeAbrio = false;

    public GestionLuz luzAzul;
    public GestionLuz luzRoja;

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
                abrirPuerta.setSeAbre();

            luzAzul.setAbierto();
            luzRoja.setAbierto();
        }
    }
}
