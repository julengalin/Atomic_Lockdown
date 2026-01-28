using UnityEngine;

public class CapsulePairTrigger : MonoBehaviour
{
    public CapsulePairSequenceController controller;

    [Tooltip("0=(1,2)  1=(3,4)  2=(5,6)  3=(7,8)")]
    public int pairIndex = 0;

    [Header("Audio")]
    public AudioClip activateSound;      // Arrastra aquí el sonido
    public AudioSource audioSource;       // Opcional
    [Range(0f, 1f)] public float volume = 1f;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;

        PlaySound();
        controller.ActivatePair(pairIndex);
    }

    private void PlaySound()
    {
        if (!activateSound) return;

        // Si hay AudioSource, lo usamos
        if (audioSource)
        {
            audioSource.PlayOneShot(activateSound, volume);
        }
        else
        {
            // Si no, sonido en la posición del trigger
            AudioSource.PlayClipAtPoint(activateSound, transform.position, volume);
        }
    }
}
