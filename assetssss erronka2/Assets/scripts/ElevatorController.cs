using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    public Transform platform;
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveSound;
    [Range(0f, 1f)] public float volume = 1f;

    private bool goingUp;
    private bool moving;

    void Awake()
    {
        // Asegurar AudioSource
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();

        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f; // 2D (evita problemas)
        audioSource.clip = moveSound;
    }

    void Update()
    {
        if (!moving || platform == null) return;

        Transform target = goingUp ? topPoint : bottomPoint;

        platform.position = Vector3.MoveTowards(
            platform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Llegó al destino
        if (Vector3.Distance(platform.position, target.position) < 0.01f)
        {
            moving = false;
            StopSound();
        }
    }

    public void ToggleElevator()
    {
        goingUp = !goingUp;
        moving = true;

        Debug.Log("Elevator -> " + (goingUp ? "SUBE" : "BAJA"));

        PlaySound();
    }

    private void PlaySound()
    {
        if (!audioSource || !moveSound) return;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    private void StopSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
