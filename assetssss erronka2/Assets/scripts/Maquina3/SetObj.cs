using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObj : MonoBehaviour
{
    [SerializeField] Transform positions;
    [SerializeField] bool left;
    [SerializeField] int count;
    [SerializeField] GestionBalanza gestionBalanza;

    readonly HashSet<Transform> inside = new HashSet<Transform>();
    Collider triggerCol;

    void Awake()
    {
        triggerCol = GetComponent<Collider>();
        if (triggerCol == null) return;

        Collider[] hits = Physics.OverlapBox(
            triggerCol.bounds.center,
            triggerCol.bounds.extents,
            triggerCol.transform.rotation
        );

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].CompareTag("Weight")) continue;
            if (inside.Contains(hits[i].transform)) continue;

            inside.Add(hits[i].transform);
            StartCoroutine(SnapNextFrame(hits[i].transform));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Weight")) return;
        if (positions == null) return;
        if (inside.Contains(other.transform)) return;

        inside.Add(other.transform);
        StartCoroutine(SnapNextFrame(other.transform));
    }

    IEnumerator SnapNextFrame(Transform t)
    {
        yield return null;

        int next = count + 1;
        string side = left ? "L" : "R";
        string targetName = "Pos" + next + side;

        Transform target = positions.Find(targetName);
        if (target == null)
        {
            inside.Remove(t);
            yield break;
        }

        t.position = target.position;
        t.rotation = target.rotation;

        Rigidbody rb = t.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        PesoObj pesoObj = t.GetComponent<PesoObj>();
        if (pesoObj != null && gestionBalanza != null)
        {
            gestionBalanza.EntrarPeso(pesoObj.getPeso(), left);
        }

        count++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Weight")) return;

        if (inside.Remove(other.transform))
        {
            count = Mathf.Max(0, count - 1);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            PesoObj pesoObj = other.GetComponent<PesoObj>();
            if (pesoObj != null && gestionBalanza != null)
            {
                gestionBalanza.SalirPeso(pesoObj.getPeso(), left);
            }
        }
    }
}
