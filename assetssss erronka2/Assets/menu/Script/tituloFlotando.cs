using UnityEngine;

public class FloatingTitle : MonoBehaviour
{
    public float floatHeight = 15f;   // cuánto sube y baja (en UI suele ser 10–20)
    public float floatSpeed = 1f;     // velocidad del movimiento

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
