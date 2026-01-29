using UnityEngine;

public class Pistola : MonoBehaviour
{
    public LineRenderer linePrefab;
    public Transform shootingPoint;
    public float maxLineDistance = 5;
    public float lineShowTimer = 0.3f;

    public DisparoTipo tipoDisparo = DisparoTipo.Rojo;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Disparar();
        }
    }

    void Disparar()
    {
        LineRenderer line = Instantiate(linePrefab);
        line.positionCount = 2;
        line.SetPosition(0, shootingPoint.position);

        Vector3 endPoint = shootingPoint.position + shootingPoint.forward * maxLineDistance;

        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxLineDistance))
        {
            IRecibeDisparo r = hit.collider.GetComponentInParent<IRecibeDisparo>();
            if (r != null) r.RecibirDisparo(tipoDisparo);

            endPoint = hit.point;
        }

        line.SetPosition(1, endPoint);
        Destroy(line.gameObject, lineShowTimer);
    }
}
