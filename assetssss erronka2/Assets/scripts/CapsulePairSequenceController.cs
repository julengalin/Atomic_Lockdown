using UnityEngine;

public class CapsulePairSequenceController : MonoBehaviour
{
    [Header("Cápsulas en orden 1..8")]
    public CapsuleUnit[] capsules;

    [Header("Capsula madre (se enciende al final)")]
    public CapsuleUnit motherCapsule;

    [Header("Inicio")]
    public bool startAllOff = true;

    private int nextPairIndex = 0;
    // 0=(1,2), 1=(3,4), 2=(5,6), 3=(7,8)

    void Start()
    {
        if (!startAllOff) return;

        // Apagar cápsulas y sus luces
        if (capsules != null)
        {
            foreach (var c in capsules)
            {
                if (!c) continue;
                c.SetOn(false);
                SetLightsInChildren(c.transform, false);
            }
        }

        if (motherCapsule)
        {
            motherCapsule.SetOn(false);
            SetLightsInChildren(motherCapsule.transform, false);
        }
    }

    // pairIndex: 0..3
    public void ActivatePair(int pairIndex)
    {
        if (capsules == null || capsules.Length < 2) return;
        if (pairIndex != nextPairIndex) return;

        int a = pairIndex * 2;
        int b = a + 1;

        ActivateCapsule(a);
        ActivateCapsule(b);

        nextPairIndex++;

        // Si ya se encendieron todas las parejas, enciende madre
        int totalPairs = capsules.Length / 2;
        if (nextPairIndex >= totalPairs && motherCapsule)
        {
            motherCapsule.SetOn(true);
            SetLightsInChildren(motherCapsule.transform, true);
        }
    }

    private void ActivateCapsule(int index)
    {
        if (index < 0 || index >= capsules.Length) return;
        if (!capsules[index]) return;

        capsules[index].SetOn(true);
        SetLightsInChildren(capsules[index].transform, true);
    }

    private void SetLightsInChildren(Transform root, bool on)
    {
        var lights = root.GetComponentsInChildren<Light>(true);
        foreach (var l in lights)
            l.enabled = on;
    }
}
