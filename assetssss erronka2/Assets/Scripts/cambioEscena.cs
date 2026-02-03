using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

public class PortalCambioEscena : MonoBehaviour
{
    public string escenaDestino = "SalaGrande";
    public float cooldownSegundos = 1f;

    // Bloqueo GLOBAL entre escenas
    private static float nextAllowedTime = 0f;

    private void OnTriggerEnter(Collider other)
    {
        // Evita disparos justo al cargar escena (spawn dentro del trigger)
        if (Time.timeSinceLevelLoad < 1f) return;

        // Cooldown global
        if (Time.time < nextAllowedTime) return;

        // XR: aceptar colisionadores hijos del XR Origin
        if (other.GetComponentInParent<XROrigin>() == null) return;

        // No recargar la misma escena en bucle
        if (SceneManager.GetActiveScene().name == escenaDestino) return;

        nextAllowedTime = Time.time + cooldownSegundos;

        // Desactiva el collider para que no re-dispare mientras carga
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        SceneManager.LoadScene(escenaDestino, LoadSceneMode.Single);
    }
}
