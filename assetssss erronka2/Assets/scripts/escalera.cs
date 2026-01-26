using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 2.5f;

    private bool isClimbing = false;
    private CharacterController controller;
    private Transform player;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;
        controller = other.GetComponent<CharacterController>();
        isClimbing = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isClimbing = false;
    }

    void Update()
    {
        if (!isClimbing || controller == null) return;

        float vertical = Input.GetAxis("Vertical"); // W/S
        Vector3 move = Vector3.up * vertical * climbSpeed;

        controller.Move(move * Time.deltaTime);
    }
}
