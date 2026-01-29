using UnityEngine;

public class Pistola : MonoBehaviour
{
    public LineRenderer linePrefab;
    public Transform shootingPoint;
    public float maxLineDistance = 5;
    public float lineShowTimer = 0.3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }
    }

    void Disparar()
    {
        Debug.Log("Disparo");

        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, shootingPoint.position);

        Vector3 endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance;

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxLineDistance))
        {

            EnemigoHit e1 = hit.collider.GetComponent<EnemigoHit>();
            EnemigoHit e2 = hit.collider.GetComponentInParent<EnemigoHit>();

            if (e2 != null) e2.RecibirDisparo();

            endPoint = hit.point;
        }
        else
        {
            Debug.Log("No hit");
        }

        line.SetPosition(1, endPoint);
        Destroy(line.gameObject, lineShowTimer);
    }
}
