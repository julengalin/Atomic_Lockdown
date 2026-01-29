using UnityEngine;

public class EnemigoHit : MonoBehaviour, IRecibeDisparo
{
    [SerializeField] int disparosNecesarios = 6;
    int disparosRecibidos;

    public GameObject card;

    [SerializeField] DisparoTipo recibeTipo = DisparoTipo.Any;

    void Awake()
    {
        if (card != null)
            card.SetActive(false);
    }

    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (recibeTipo != DisparoTipo.Any && tipo != recibeTipo)
            return;

        Debug.Log("Disparo recibido raycast");

        disparosRecibidos++;

        if (disparosRecibidos >= disparosNecesarios)
        {
            if (card != null)
                card.SetActive(true);

            Destroy(gameObject);
        }
    }
}
