using UnityEngine;

public class GestionBalanza : MonoBehaviour
{
    float weightL;
    float weightR;

    public void EntrarPeso(float peso, bool left)
    {
        if (left)
            weightL += peso;
        else
            weightR += peso;

        Debug.Log("Peso L: " + weightL + " | Peso R: " + weightR);
    }

    public void SalirPeso(float peso, bool left)
    {
        if (left)
            weightL -= peso;
        else
            weightR -= peso;

        Debug.Log("Peso L: " + weightL + " | Peso R: " + weightR);
    }
}
