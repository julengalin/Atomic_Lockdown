using UnityEngine;

public class EnemigoHit : MonoBehaviour
{
    [SerializeField] int disparosNecesarios = 6;
    int disparosRecibidos;

    public void RecibirDisparo()
    {
        Debug.Log("Disparo recibido raycast");

        disparosRecibidos++;

        if (disparosRecibidos >= disparosNecesarios)
        {
            Destroy(gameObject);
        }
    }
}
