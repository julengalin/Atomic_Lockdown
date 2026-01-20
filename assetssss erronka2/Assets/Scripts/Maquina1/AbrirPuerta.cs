using UnityEngine;

public class AbrirPuerta : MonoBehaviour
{

    [SerializeField] Animator animator;
    [SerializeField] InteractionLock interactionLock;
    public InteractionType tipo;
    [SerializeField] Camera camAnim;

    bool abierto;

    public void Abrir()
    {
        if (abierto || animator == null || camAnim == null) return;

        interactionLock.Set(InteractionType.Cinematica);

        camAnim.enabled = true;
        var animListener = camAnim.GetComponent<AudioListener>();
        if (animListener != null) animListener.enabled = true;

        var cams = Object.FindObjectsByType<Camera>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (var cam in cams)
        {
            if (cam == camAnim) continue;

            cam.enabled = false;
            var al = cam.GetComponent<AudioListener>();
            if (al != null) al.enabled = false;
        }

        animator.Play("Abrir");
        abierto = true;
    }

    public void ReactivarCamarasJugador()
    {
        var players = GameObject.FindGameObjectsWithTag("PlayerCam");

        foreach (var go in players)
        {
            var cam = go.GetComponent<Camera>();
            if (cam != null) cam.enabled = true;

            var al = go.GetComponent<AudioListener>();
            if (al != null) al.enabled = true;
        }

        if (camAnim != null)
        {
            camAnim.enabled = false;
            var al = camAnim.GetComponent<AudioListener>();
            if (al != null) al.enabled = false;
        }

        interactionLock.Limpiar();
    }
}
