using UnityEngine;

public class LightFlickerSimple : MonoBehaviour
{
    public Light flickerLight;

    public float minOnTime = 0.05f;
    public float maxOnTime = 0.2f;

    public float minOffTime = 0.4f;
    public float maxOffTime = 1.2f;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            // Apagar
            flickerLight.enabled = false;
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));

            // Encender
            flickerLight.enabled = true;
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));
        }
    }
}
