using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LevitateOnly : MonoBehaviour
{
    [Header("Levitación (con físicas)")]
    public float amplitude = 0.12f;
    public float speed = 0.6f;
    public bool randomPhase = true;

    private Rigidbody rb;
    private Vector3 startPos;
    private float phase;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startPos = rb.position;
        phase = randomPhase ? Random.Range(0f, 999f) : 0f;
    }

    void FixedUpdate()
    {
        float yOffset = Mathf.Sin((Time.time + phase) * speed) * amplitude;
        Vector3 targetPos = startPos + new Vector3(0f, yOffset, 0f);

        rb.MovePosition(targetPos);
    }
}
