using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public ElevatorController elevator;

    [Header("Audio")]
    public AudioClip buttonSound;
    [Range(0f, 1f)] public float volume = 1f;

    private AudioSource audioSource;
    private bool playerInside;

    void Awake()
    {
        // Siempre aseguramos un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D para evitar problemas
        audioSource.volume = volume;
    }

    void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pulsada dentro del botón");

            PlaySound();
            elevator.ToggleElevator();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player dentro del botón");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player fuera del botón");
        }
    }

    void PlaySound()
    {
        if (!buttonSound)
        {
            Debug.LogWarning("ElevatorButton: No hay AudioClip asignado.");
            return;
        }

        audioSource.Stop(); // por si acaso
        audioSource.clip = buttonSound;
        audioSource.volume = volume;
        audioSource.Play();
    }

    void OnGUI()
    {
        if (playerInside)
            GUI.Label(new Rect(20, 20, 400, 30), "Klikatu E igogailua erabiltzeko");
    }
}
