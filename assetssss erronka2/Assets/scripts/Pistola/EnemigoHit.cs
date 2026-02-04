using UnityEngine;

public class EnemigoHit : MonoBehaviour, IRecibeDisparo
{
    [Header("Requisitos")]
    [SerializeField] int disparosPorColor = 3;

    int disparosRojos = 0;
    int disparosAzules = 0;

    public GameObject card;

    void Awake()
    {
        if (card != null)
            card.SetActive(false);
    }

    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (tipo == DisparoTipo.Rojo)
            disparosRojos++;
        else if (tipo == DisparoTipo.Azul)
            disparosAzules++;
        else
            return;

        Debug.Log($"Rojos: {disparosRojos} | Azules: {disparosAzules}");

        // Solo muere si AMBOS colores cumplen
        if (disparosRojos >= disparosPorColor &&
            disparosAzules >= disparosPorColor)
        {
            if (card != null)
                card.SetActive(true);

            Destroy(gameObject);
        }
    }
}
