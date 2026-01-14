using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float sprintSpeed = 6.0f;
    public float gravity = -18f;
    public float jumpHeight = 1.2f;

    [Header("Mouse Look")]
    public Transform cameraRoot;          // un empty a la altura de la cabeza
    public Camera playerCamera;           // la cámara (Main Camera o una nueva)
    public float mouseSensitivity = 2.0f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Ground Check")]
    public float groundStickForce = -2f;  // mantiene pegado al suelo

    CharacterController cc;
    float yVelocity;
    float pitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        // Si no asignas nada, intenta auto-encontrar
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        if (cameraRoot == null && playerCamera != null) cameraRoot = playerCamera.transform.parent;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal (cuerpo)
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical (cámara)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraRoot != null)
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        else if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Move()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        float x = Input.GetAxisRaw("Horizontal"); // A/D
        float z = Input.GetAxisRaw("Vertical");   // W/S
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        // Gravedad y suelo
        if (cc.isGrounded && yVelocity < 0f)
            yVelocity = groundStickForce;

        // Salto (opcional)
        if (cc.isGrounded && Input.GetKeyDown(KeyCode.Space))
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed;
        velocity.y = yVelocity;

        cc.Move(velocity * Time.deltaTime);
    }
}
