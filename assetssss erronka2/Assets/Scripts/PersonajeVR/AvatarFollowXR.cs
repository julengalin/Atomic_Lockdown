using UnityEngine;

public class AvatarFollowXR : MonoBehaviour
{
    [Header("Referencias")]
    public Transform xrOrigin;      // XR Origin
    public Transform head;          // Main Camera (HMD)

    [Header("Rotación del cuerpo")]
    public float rotationSpeed = 8f;

    void LateUpdate()
    {
        if (!xrOrigin || !head) return;

        // Posición: el avatar sigue al XR Origin
        transform.position = xrOrigin.position;

        // Dirección de la cabeza (solo en plano horizontal)
        Vector3 forward = head.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }
}
