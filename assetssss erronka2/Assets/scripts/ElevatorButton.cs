using UnityEngine;
using UnityEngine.XR;

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
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = volume;
    }

    void Update()
    {
        if (!playerInside) return;

        // Leer mando derecho (Quest)
        InputDevice rightHand =
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        // Gatillo trasero (Grip Button)
        bool gripPressed = false;

        if (rightHand.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed)
            && gripPressed)
        {
            Debug.Log("Grip derecho pulsado dentro del botón");

            PlaySound();
            elevator.ToggleElevator();

            // Evitar que se active cada frame mientras lo mantienes pulsado
            playerInside = false;
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

        audioSource.Stop();
        audioSource.clip = buttonSound;
        audioSource.volume = volume;
        audioSource.Play();
    }

    void OnGUI()
    {
        if (playerInside)
            GUI.Label(new Rect(20, 20, 400, 30),
                "Pulsa el Grip derecho para usar el ascensor");
    }
}
