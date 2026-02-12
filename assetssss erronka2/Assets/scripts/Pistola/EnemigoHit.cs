using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class EnemigoHit : NetworkBehaviour, IRecibeDisparo
{
    [Header("Requisitos")]
    [SerializeField] private int disparosPorColor = 2;

    [Header("Animación")]
    public Enemy_NoNavMesh enemyAI;
    public float deathDelay = 2.0f; // duración animación

    [Header("UI/Drop")]
    public GameObject card;

    private NetworkVariable<int> disparosRojos = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> disparosAzules = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<bool> muerto = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake()
    {
        if (!enemyAI) enemyAI = GetComponent<Enemy_NoNavMesh>();
        if (!enemyAI) enemyAI = GetComponentInChildren<Enemy_NoNavMesh>(true);

        if (card != null)
            card.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        muerto.OnValueChanged += OnMuertoChanged;
        OnMuertoChanged(false, muerto.Value);
    }

    public override void OnNetworkDespawn()
    {
        muerto.OnValueChanged -= OnMuertoChanged;
    }

    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (!IsServer) return;
        if (muerto.Value) return;

        if (tipo == DisparoTipo.Rojo)
            disparosRojos.Value++;
        else if (tipo == DisparoTipo.Azul)
            disparosAzules.Value++;
        else
            return;

        if (disparosRojos.Value >= disparosPorColor &&
            disparosAzules.Value >= disparosPorColor)
        {
            muerto.Value = true;

            // 🔥 Ejecuta animación en todos
            DieClientRpc();

            // (Opcional) mostrar card
            ShowCardClientRpc();

            // Espera y luego elimina en red
            StartCoroutine(DespawnAfterDelay());
        }
    }

    private void OnMuertoChanged(bool oldValue, bool newValue)
    {
        if (card != null)
            card.SetActive(newValue);
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        if (enemyAI != null)
            enemyAI.Die();
    }

    [ClientRpc]
    private void ShowCardClientRpc()
    {
        if (card != null)
            card.SetActive(true);
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);

        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn(true);
        else
            Destroy(gameObject);
    }
}
