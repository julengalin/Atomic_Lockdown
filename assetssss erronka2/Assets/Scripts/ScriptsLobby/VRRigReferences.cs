using UnityEngine;

public class VRRigReferences : MonoBehaviour
{

    public static VRRigReferences Singleton;

    public Transform root;
    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    private void Awake()
    {
        Singleton = this;
            
    }
}
