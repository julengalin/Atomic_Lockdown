using UnityEngine;
using Unity.Netcode;

public class EnemigoHit : NetworkBehaviour, IRecibeDisparo
{
    [Header("Requisitos")]
    [SerializeField] private int disparosPorColor = 0;

    [Header("UI/Drop")]
    public GameObject card;

    // Contadores sincronizados
    private NetworkVariable<int> disparosRojos = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<int> disparosAzules = new(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Estado de muerto para no repetir lógica
    private NetworkVariable<bool> muerto = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    void Awake()
    {
        if (card != null)
            card.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        // Asegura que el card se actualice en clientes cuando cambie "muerto"
        muerto.OnValueChanged += OnMuertoChanged;

        // Si spawnea ya muerto (por late join), aplica estado
        OnMuertoChanged(false, muerto.Value);
    }

    public override void OnNetworkDespawn()
    {
        muerto.OnValueChanged -= OnMuertoChanged;
    }

    // Esto lo llama el servidor desde el raycast del arma (ideal)
    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (!IsServer) return;          // 🔥 autoridad en servidor
        if (muerto.Value) return;

        if (tipo == DisparoTipo.Rojo)
            disparosRojos.Value++;
        else if (tipo == DisparoTipo.Azul)
            disparosAzules.Value++;
        else
            return;

        Debug.Log($"[SERVER] Rojos: {disparosRojos.Value} | Azules: {disparosAzules.Value}");

        if (disparosRojos.Value >= disparosPorColor &&
            disparosAzules.Value >= disparosPorColor)
        {
            muerto.Value = true;

            // (Opcional) si quieres que el card se vea antes de destruir
            // lo activamos por RPC (ver abajo) y destruimos un pelín después:
            ShowCardClientRpc();

            // Destruir en red (para todos)
            if (NetworkObject != null && NetworkObject.IsSpawned)
                NetworkObject.Despawn(true);
            else
                Destroy(gameObject);
        }
    }

    private void OnMuertoChanged(bool oldValue, bool newValue)
    {
        // Esto corre en todos (server y clientes)
        if (card != null)
            card.SetActive(newValue);
    }

    [ClientRpc]
    private void ShowCardClientRpc()
    {
        if (card != null)
            card.SetActive(true);
    }
}
