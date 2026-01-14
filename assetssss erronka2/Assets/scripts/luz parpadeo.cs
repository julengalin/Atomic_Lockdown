using UnityEngine;

public class LightFlickerSimple : MonoBehaviour
{
    public Light flickerLight;
    public float minTime = 0.05f;
    public float maxTime = 0.3f;

    void Start()
    {
        StartCoroutine(Flicker());
    }

    System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            flickerLight.enabled = !flickerLight.enabled;
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
