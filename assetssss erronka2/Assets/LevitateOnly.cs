using UnityEngine;

public class LevitateOnly : MonoBehaviour
{
    [Header("Levitación (sin rotación)")]
    public float amplitude = 0.12f; // cuánto sube/baja
    public float speed = 0.6f;      // qué tan rápido

    [Header("Aleatorio")]
    public bool randomPhase = true;

    private Vector3 startPos;
    private Quaternion startRot;
    private float phase;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        phase = randomPhase ? Random.Range(0f, 999f) : 0f;
    }

    void Update()
    {
        // Mantener rotación fija siempre
        transform.rotation = startRot;

        // Solo subir/bajar en Y
        float yOffset = Mathf.Sin((Time.time + phase) * speed) * amplitude;
        transform.position = startPos + new Vector3(0f, yOffset, 0f);
    }
}
