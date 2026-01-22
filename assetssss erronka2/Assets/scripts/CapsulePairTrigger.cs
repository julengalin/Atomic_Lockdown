using UnityEngine;

public class CapsulePairTrigger : MonoBehaviour
{
    public CapsulePairSequenceController controller;
    [Tooltip("0=(1,2)  1=(3,4)  2=(5,6)  3=(7,8)")]
    public int pairIndex = 0;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        controller.ActivatePair(pairIndex);
    }
}
