using UnityEngine;

public class LightOffByDistance : MonoBehaviour
{
    public Transform player;
    public float offDistance = 3f;

    Light targetLight;

    void Awake()
    {
        targetLight = GetComponent<Light>();
    }

    void Update()
    {
        if (player == null || targetLight == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        targetLight.enabled = distance > offDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, offDistance);
    }
}
