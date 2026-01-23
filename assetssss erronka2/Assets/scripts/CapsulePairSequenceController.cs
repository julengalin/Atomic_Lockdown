using UnityEngine;

public class CapsulePairSequenceController : MonoBehaviour
{
    [Header("Cápsulas en orden 1..8")]
    public CapsuleUnit[] capsules;   // tamaño 8

    [Header("Capsula madre (se enciende al final)")]
    public CapsuleUnit motherCapsule;

    [Header("Inicio")]
    public bool startAllOff = true;

    private int nextPairIndex = 0; // 0 = (1,2), 1 = (3,4), 2=(5,6), 3=(7,8)

    void Start()
    {
        if (!startAllOff) return;

        if (capsules != null)
            foreach (var c in capsules)
                if (c) c.SetOn(false);

        if (motherCapsule) motherCapsule.SetOn(false);
    }

    // pairIndex: 0..3
    public void ActivatePair(int pairIndex)
    {
        if (capsules == null || capsules.Length < 2) return;

        // Solo deja activar la siguiente pareja (evita saltos)
        if (pairIndex != nextPairIndex) return;

        int a = pairIndex * 2;
        int b = a + 1;

        if (a >= 0 && a < capsules.Length && capsules[a]) capsules[a].SetOn(true);
        if (b >= 0 && b < capsules.Length && capsules[b]) capsules[b].SetOn(true);

        nextPairIndex++;

        // Si ya se encendieron todas las parejas, enciende madre
        int totalPairs = capsules.Length / 2;
        if (nextPairIndex >= totalPairs)
        {
            if (motherCapsule) motherCapsule.SetOn(true);
        }
    }
}
