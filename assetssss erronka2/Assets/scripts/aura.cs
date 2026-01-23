using UnityEngine;

public class ShadowPulse : MonoBehaviour
{
    public float speed = 2f;
    public float minScale = 138f;
    public float maxScale = 150f;

    void Update()
    {
        float s = Mathf.Lerp(minScale, maxScale,
            (Mathf.Sin(Time.time * speed) + 1f) * 0.5f);

        transform.localScale = new Vector3(s, s * 0.9f, s);
    }
}
