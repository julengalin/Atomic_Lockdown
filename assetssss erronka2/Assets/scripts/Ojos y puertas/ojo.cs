using UnityEngine;

public class EyeFollowCenter : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public bool onlyYaw = true;
    public float turnSpeed = 10f;

    [Header("Mesh Renderer (del ojo)")]
    public Renderer rend;   // arrastra aquí el Renderer de Eye_2_R

    void Update()
    {
        if (!player || !rend) return;

        // Centro real del mesh en mundo (similar a "Center" del editor)
        Vector3 center = rend.bounds.center;

        // Punto al que mirar
        Vector3 target = player.position;
        if (onlyYaw) target.y = center.y;

        Vector3 dir = target - center;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        // Rotar el objeto alrededor del centro del mesh, no del pivot
        Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        Quaternion delta = newRot * Quaternion.Inverse(transform.rotation);

        transform.rotation = newRot;

        // Compensa la posición para que el centro del mesh se mantenga en su sitio
        Vector3 newCenter = center;
        Vector3 offset = center - transform.position;
        Vector3 rotatedOffset = delta * offset;

        transform.position += (offset - rotatedOffset);
    }
}
