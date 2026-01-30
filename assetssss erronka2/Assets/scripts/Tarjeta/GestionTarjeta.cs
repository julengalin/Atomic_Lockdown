using UnityEngine;
using UnityEngine.UI;

public class GestionTarjeta : MonoBehaviour
{
    [SerializeField] Image iconoTarjeta;
    bool picked;

    public void RecogerTarjeta()
    {
        picked = true;
        if (iconoTarjeta != null)
            iconoTarjeta.gameObject.SetActive(true);
    }

    public bool TieneTarjeta()
    {
        return picked;
    }

    public void ConsumirTarjeta()
    {
        picked = false;
        if (iconoTarjeta != null)
            iconoTarjeta.gameObject.SetActive(false);
    }
}
