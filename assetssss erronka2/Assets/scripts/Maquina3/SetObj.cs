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
    readonly HashSet<Transform> snapped = new HashSet<Transform>();
    readonly Dictionary<Transform, Transform> originalParent = new Dictionary<Transform, Transform>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Weight")) return;
        if (inside.Contains(other.transform)) return;

        inside.Add(other.transform);
        StartCoroutine(SnapNextFrame(other.transform));
    }

    IEnumerator SnapNextFrame(Transform t)
    {
        yield return null;

        PesoObj pesoObj = t.GetComponent<PesoObj>();
        if (pesoObj == null)
        {
            inside.Remove(t);
            yield break;
        }

        if (!pesoObj.esFijo)
        {
            if (positions == null)
            {
                inside.Remove(t);
                yield break;
            }

            int next = count + 1;
            string side = left ? "L" : "R";
            string targetName = "Pos" + next + side;

            Transform target = positions.Find(targetName);
            if (target == null)
            {
                inside.Remove(t);
                yield break;
            }

            if (!originalParent.ContainsKey(t))
                originalParent[t] = t.parent;

            t.SetParent(target, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;

            Rigidbody rb = t.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            snapped.Add(t);
            count++;
        }

        if (gestionBalanza != null)
        {
            gestionBalanza.EntrarPeso(pesoObj.getPeso(), left);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Weight")) return;

        Transform t = other.transform;

        if (!inside.Remove(t)) return;

        PesoObj pesoObj = other.GetComponent<PesoObj>();
        if (pesoObj != null && gestionBalanza != null)
        {
            gestionBalanza.SalirPeso(pesoObj.getPeso(), left);
        }

        if (pesoObj != null && !pesoObj.esFijo)
        {
            snapped.Remove(t);
            count = Mathf.Max(0, count - 1);

            if (originalParent.TryGetValue(t, out Transform p))
            {
                t.SetParent(p, true);
                originalParent.Remove(t);
            }
            else
            {
                t.SetParent(null, true);
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = false;
        }
        else
        {
            snapped.Remove(t);
        }
    }
}
