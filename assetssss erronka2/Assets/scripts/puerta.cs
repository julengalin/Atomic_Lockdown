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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound; // opcional

    private Vector3 closedPos;
    private Vector3 openPos;

    private bool isMoving;
    private bool isOpen;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + slideDirection.normalized * slideAmount;

        // Seguridad: si no asignas AudioSource, lo busca en la puerta
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!player || isMoving || isOpen)
            return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= openDistance)
        {
            StartCoroutine(OpenDoorRoutine());
        }
    }

    IEnumerator OpenDoorRoutine()
    {
        isMoving = true;

        // 🔊 SONIDO DE ABRIR
        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound);

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

        // 🔊 SONIDO DE CERRAR (opcional)
        if (audioSource && closeSound)
            audioSource.PlayOneShot(closeSound);

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
