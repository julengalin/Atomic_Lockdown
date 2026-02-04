using UnityEngine;
using TMPro;

public class TitleFlicker : MonoBehaviour
{
    public TMP_Text text;
    public float minAlpha = 0.8f;
    public float maxAlpha = 1f;
    public float speed = 2f;

    void Update()
    {
        Color c = text.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * speed, 1));
        text.color = c;
    }
}
