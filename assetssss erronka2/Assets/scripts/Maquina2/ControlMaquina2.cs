using UnityEngine;

public class ControlMaquina2 : MonoBehaviour
{
    public int[] ordenCorrecto = new int[] { 1, 3, 2 };
    public int[] ordenJugadores = new int[] { 0, 0, 0 };

    public float fillCorrecto1;
    public float fillCorrecto2;
    public float fillCorrecto3;

    public float fillJugadores1;
    public float fillJugadores2;
    public float fillJugadores3;

    [SerializeField] float toleranciaFill = 0.01f;
    [SerializeField] ResetPalancas resetPalancas;

    public Animator animator;

    public void UpdateList(int id, float fill)
    {
        for (int i = 0; i < ordenJugadores.Length; i++)
        {
            if (ordenJugadores[i] == 0)
            {
                ordenJugadores[i] = id;
                break;
            }
        }

        if (id == 1) fillJugadores1 = fill;
        else if (id == 2) fillJugadores2 = fill;
        else if (id == 3) fillJugadores3 = fill;

        bool ordenCompleto = ordenJugadores[0] != 0 && ordenJugadores[1] != 0 && ordenJugadores[2] != 0;
        bool fillCompleto = fillJugadores1 != 0f && fillJugadores2 != 0f && fillJugadores3 != 0f;

        if (!ordenCompleto || !fillCompleto) return;

        bool ordenOk =
            ordenJugadores[0] == ordenCorrecto[0] &&
            ordenJugadores[1] == ordenCorrecto[1] &&
            ordenJugadores[2] == ordenCorrecto[2];

        bool fillOk =
            Mathf.Abs(fillJugadores1 - fillCorrecto1) <= toleranciaFill &&
            Mathf.Abs(fillJugadores2 - fillCorrecto2) <= toleranciaFill &&
            Mathf.Abs(fillJugadores3 - fillCorrecto3) <= toleranciaFill;

        if (ordenOk && fillOk)
        {
            animator.Play("Abrir");
        }
        else
        {
            if (resetPalancas != null) resetPalancas.Resetear();
            ResetearEstado();
        }
    }

    public void ResetearEstado()
    {
        ordenJugadores[0] = 0;
        ordenJugadores[1] = 0;
        ordenJugadores[2] = 0;

        fillJugadores1 = 0f;
        fillJugadores2 = 0f;
        fillJugadores3 = 0f;
    }
}
