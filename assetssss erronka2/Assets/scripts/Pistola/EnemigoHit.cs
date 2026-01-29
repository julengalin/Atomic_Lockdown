using UnityEngine;

public class EnemigoHit : MonoBehaviour
{
    [SerializeField] int disparosNecesarios = 6;
    int disparosRecibidos;
    public GameObject card;

    private void Awake()
    {
        card.SetActive(false);
    }

    public void RecibirDisparo()
    {
        Debug.Log("Disparo recibido raycast");

        disparosRecibidos++;

        if (disparosRecibidos >= disparosNecesarios)
        {
            Destroy(gameObject);
            card.SetActive(true);
        }
    }
}
