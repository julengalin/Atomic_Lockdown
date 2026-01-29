using UnityEngine;

public class LectorTarjetaProxy : MonoBehaviour
{
    [SerializeField] LectorTarjeta lectorTarjeta;

    public void AbrirPuerta()
    {
        if (lectorTarjeta != null)
            lectorTarjeta.AbrirPuerta();
    }
}
