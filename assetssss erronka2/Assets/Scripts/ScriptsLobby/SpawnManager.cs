using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] private Transform[] spawnPoints;

    private int nextSpawnIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetNextSpawnPoint()
    {
        if (spawnPoints.Length == 0)
            return null;

        Transform point = spawnPoints[nextSpawnIndex];
        nextSpawnIndex = (nextSpawnIndex + 1) % spawnPoints.Length;
        return point;
    }
}
