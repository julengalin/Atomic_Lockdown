using System.Collections;
using UnityEngine;

public class SpawnPlayerSalaGrande : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private IEnumerator Start()
    {
        yield return null; // esperar 1 frame

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && spawnPoint != null)
        {
            var cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

            if (cc) cc.enabled = true;
        }
    }
}
