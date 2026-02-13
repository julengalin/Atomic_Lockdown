using UnityEngine;

public class GestionLuz : MonoBehaviour
{
    public Light luz;

    public bool frio = false;

    public float intensidadBase = 0.3f;
    public float intensidadMax = 3f;

    public int maxDiff = 5;

    bool abierto = false;

    public void SetDiff(int diff)
    {
        Debug.Log("Entramos en set diff y la diff es = " +  diff.ToString());
        if (luz == null) return;

        Debug.Log("Luz existe");

        int abs = Mathf.Abs(diff);

        float t = 0f;
        if (maxDiff > 0) t = Mathf.Clamp01(abs / (float)maxDiff);

        float intensidad = Mathf.Lerp(intensidadBase, intensidadMax, t);

        Debug.Log("intensidad =" + intensidad);

        Color c;

        if (diff == 0)
        {
            c = Color.green;
            intensidad = intensidadBase;
        }
        else
        {
            if (abierto) return;
            bool rojo = diff > 0;

            if (frio) rojo = !rojo;

            c = rojo ? Color.red : Color.blue;
        }

        Debug.Log("Color =" + c);
        luz.color = c;
        luz.intensity = intensidad;
    }

    public void setAbierto()
    {
        abierto = true;
    }
}
