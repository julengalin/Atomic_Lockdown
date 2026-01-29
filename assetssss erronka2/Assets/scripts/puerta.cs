using UnityEngine;
using System.Collections;

public class SlidingDoor : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float openDistance = 2.0f;

    [Header("Door Movement")]
    public Vector3 slideDirection = Vector3.right;
    public float slideAmount = 1.5f;
    public float speed = 2.5f;

    [Header("Timing")]
    public float stayOpenTime = 3f;

    public bool blocked = false;

    private Vector3 closedPos;
    private Vector3 openPos;

    private bool isMoving;
    private bool isOpen;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + slideDirection.normalized * slideAmount;
    }

    void Update()
    {
        if (blocked)
            return;

        if (!player || isMoving || isOpen)
            return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= openDistance)
        {
            StartCoroutine(OpenDoorRoutine());
        }
    }

    public void Unlock()
    {
        blocked = false;
    }

    public void ForceOpen()
    {
        blocked = false;

        if (isMoving || isOpen)
            return;

        StartCoroutine(OpenDoorRoutine());
    }

    IEnumerator OpenDoorRoutine()
    {
        isMoving = true;

        // ABRIR
        while (Vector3.Distance(transform.position, openPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, openPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = openPos;
        isOpen = true;
        isMoving = false;

        // ESPERAR ABIERTA
        yield return new WaitForSeconds(stayOpenTime);

        // CERRAR
        isMoving = true;

        while (Vector3.Distance(transform.position, closedPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, closedPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = closedPos;
        isOpen = false;
        isMoving = false;
    }
}
