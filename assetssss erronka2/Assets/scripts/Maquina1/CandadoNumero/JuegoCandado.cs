using UnityEngine;

public class JuegoCandado : MonoBehaviour
{
    [SerializeField] private AbrirCandado AbrirCandado;
    [SerializeField] private GameObject tubo;
    [SerializeField] private GameObject rueda1;
    [SerializeField] private GameObject rueda2;
    [SerializeField] private GameObject rueda3;

    [SerializeField] private Animator tuboAnimator;

    public bool ended = false;

    public float playPos;
    public float endPos;

    [SerializeField] private int[] correctNumbers;
    [SerializeField] private int[] actualNumbers;

    public ControlMaquina1 controlMaquina;

    private void Update()
    {
        if (AbrirCandado.playMode)
        {
            Vector3 p = tubo.transform.localPosition;

            tubo.transform.localPosition = p;
        }
    }

    public void updatePos(int pos)
    {
        actualNumbers[pos]++;
        if (actualNumbers[pos] >= 10) actualNumbers[pos] = 0;
    }

    public void check()
    {

        int correct = 0;
        for (int i = 0; i < correctNumbers.Length; i++)
        {
            if (correctNumbers[i] == actualNumbers[i])
                correct++;
        }

        if (correct == correctNumbers.Length)
        {
            ended = true;

            if (tuboAnimator != null)
                tuboAnimator.enabled = true;
                AbrirCandado.MarcarAbierto();
                tuboAnimator.Play("Abierto");

            controlMaquina.SetAzul();
        }
        else
        {
            rueda1.GetComponent<RotateNumbers>().ResetWheel();
            rueda2.GetComponent<RotateNumbers>().ResetWheel();
            rueda3.GetComponent<RotateNumbers>().ResetWheel();
        }
    }
}