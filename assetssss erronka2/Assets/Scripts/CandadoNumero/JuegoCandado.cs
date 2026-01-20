using UnityEngine;

public class JuegoCandado : MonoBehaviour
{

    [SerializeField] private AbrirCandado AbrirCandado;
    [SerializeField] private GameObject tubo;
    [SerializeField] private GameObject rueda1;
    [SerializeField] private GameObject rueda2;
    [SerializeField] private GameObject rueda3;

    public bool ended = false;

    public float playPos;
    public float endPos;

    [SerializeField] private int[] correctNumbers;
    [SerializeField] private int[] actualNumbers;

    public ControlMaquina controlMaquina;


    private void Update()
    {
        if (AbrirCandado.playMode)
        {
            Vector3 p = tubo.transform.localPosition;

            if (!ended) p.y = playPos;
            else p.y = endPos;

            tubo.transform.localPosition = p;
        }
    }


    public void updatePos(int pos)
    {
        actualNumbers[pos]++;
        if (actualNumbers[pos] >= 10) actualNumbers[pos] = 0;
        Debug.Log(actualNumbers[pos].ToString());
    }

    public void check()
    {

        int correct = 0;
        for (int i = 0; i < correctNumbers.Length; i++)
        {
            if (correctNumbers[i] == actualNumbers[i])
            {
                correct++;
            }
        }

        Debug.Log(correct.ToString());

        if (correct == correctNumbers.Length)
        {
            Debug.Log("correct");
            ended = true;
            controlMaquina.SetAzul();
        }
        else
        {
            Debug.Log("incorrect");
            rueda1.gameObject.GetComponent<RotateNumbers>().ResetWheel();
            rueda2.gameObject.GetComponent<RotateNumbers>().ResetWheel();
            rueda3.gameObject.GetComponent<RotateNumbers>().ResetWheel();
        }
    }

}
