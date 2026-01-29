using UnityEngine;

public class DoorEyeLock : MonoBehaviour
{
    [SerializeField] SlidingDoor door;
    [SerializeField] int eyesRequired = 1;

    int eyesRemaining;

    void Awake()
    {
        eyesRemaining = Mathf.Max(0, eyesRequired);

        if (door != null && eyesRemaining > 0)
            door.blocked = true;
    }

    public void OjoMuerto()
    {
        if (eyesRemaining <= 0) return;

        eyesRemaining--;

        if (eyesRemaining <= 0 && door != null)
        {
            door.blocked = false;
            door.ForceOpen();
        }
    }
}
