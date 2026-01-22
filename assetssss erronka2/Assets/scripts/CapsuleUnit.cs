using UnityEngine;

public class CapsuleUnit : MonoBehaviour
{
    [Header("Luces de la cápsula (arriba/abajo)")]
    public Light topLight;
    public Light bottomLight;

    [Header("Audio (opcional)")]
    public AudioSource humAudio;

    public void SetOn(bool on)
    {
        if (topLight) topLight.enabled = on;
        if (bottomLight) bottomLight.enabled = on;

        if (humAudio)
        {
            if (on && !humAudio.isPlaying) humAudio.Play();
            if (!on && humAudio.isPlaying) humAudio.Stop();
        }
    }
}
