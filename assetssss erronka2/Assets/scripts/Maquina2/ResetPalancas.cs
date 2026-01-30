using UnityEngine;

public class ResetPalancas : MonoBehaviour
{
    [SerializeField] GirarPalanca[] palancas;

    private void OnMouseDown()
    {
        Resetear();
    }

    public void Resetear()
    {
        if (palancas != null && palancas.Length > 0)
        {
            for (int i = 0; i < palancas.Length; i++)
            {
                if (palancas[i] != null) palancas[i].ResetearPalanca();
            }
            return;
        }

        var found = Object.FindObjectsByType<GirarPalanca>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < found.Length; i++)
        {
            if (found[i] != null) found[i].ResetearPalanca();
        }
    }
}
