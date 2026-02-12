using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class SlidingDoor : NetworkBehaviour
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
    public AudioClip closeSound;

    private Vector3 closedPos;
    private Vector3 openPos;

    private bool isMoving;
    private bool isOpen;

    private NetworkVariable<bool> blocked = new(
    false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public bool IsBlocked => blocked.Value;

    public void SetBlocked(bool value)
    {
        if (!IsServer) return;
        blocked.Value = value;
    }

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + slideDirection.normalized * slideAmount;

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!IsServer) return; // 🔥 SOLO el servidor controla

        if (blocked.Value)
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
        if (!IsServer) return;
        blocked.Value = false;
    }

    public void ForceOpen()
    {
        if (!IsServer) return;

        blocked.Value = false;

        if (isMoving || isOpen)
            return;

        StartCoroutine(OpenDoorRoutine());
    }

    IEnumerator OpenDoorRoutine()
    {
        isMoving = true;

        PlaySoundClientRpc(true);

        // ABRIR
        while (Vector3.Distance(transform.position, openPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                openPos,
                speed * Time.deltaTime);

            yield return null;
        }

        transform.position = openPos;
        isOpen = true;
        isMoving = false;

        yield return new WaitForSeconds(stayOpenTime);

        PlaySoundClientRpc(false);

        // CERRAR
        isMoving = true;

        while (Vector3.Distance(transform.position, closedPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                closedPos,
                speed * Time.deltaTime);

            yield return null;
        }

        transform.position = closedPos;
        isOpen = false;
        isMoving = false;
    }

    [ClientRpc]
    private void PlaySoundClientRpc(bool opening)
    {
        if (!audioSource) return;

        if (opening && openSound)
            audioSource.PlayOneShot(openSound);
        else if (!opening && closeSound)
            audioSource.PlayOneShot(closeSound);
    }
}