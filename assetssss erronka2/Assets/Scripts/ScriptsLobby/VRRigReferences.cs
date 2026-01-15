using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences Singleton { get; private set; }

    public Transform root;
    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            // Si aparece otro XR Rig por recarga/sync, no machacamos el singleton
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject); // clave si Netcode hace scene sync
        Debug.Log("VRRigReferences set ✅ (DontDestroyOnLoad)");
    }

    private void OnDestroy()
    {
        if (Singleton == this) Singleton = null;
    }
}
