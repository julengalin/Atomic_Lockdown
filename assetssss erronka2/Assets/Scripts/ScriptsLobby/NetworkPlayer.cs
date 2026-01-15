using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Avatar Transforms (children of this prefab)")]
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Hide local meshes")]
    public Renderer[] meshToDisable;

    [Header("Net Settings")]
    [Tooltip("How many pose updates per second the owner sends.")]
    [SerializeField] private float sendRateHz = 30f;

    [Tooltip("Remote smoothing speed (bigger = snappier).")]
    [SerializeField] private float lerpSpeed = 20f;

    private VRRigReferences rig;
    private float _sendTimer;

    // Networked pose (root world, head/hands local-to-root)
    private NetworkVariable<Vector3> rootPos =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> rootRot =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> headPos =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> headRot =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> leftPos =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> leftRot =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> rightPos =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> rightRot =
        new(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            CacheRig();

            foreach (var r in meshToDisable)
                if (r) r.enabled = false;

            Debug.Log($"[NetworkPlayer] OWNER spawned. Rig={(rig != null ? "OK" : "NULL")}");
        }
        else
        {
            Debug.Log($"[NetworkPlayer] REMOTE spawned. OwnerClientId={OwnerClientId}");
        }
    }

    private void CacheRig()
    {
        rig = VRRigReferences.Singleton;
    }

    private void LateUpdate()
    {
        if (!IsSpawned) return;

        if (IsOwner)
        {
            // Re-cache por si el rig se recrea / cambia de escena / etc.
            if (rig == null || rig.root == null || rig.head == null || rig.leftHand == null || rig.rightHand == null)
                CacheRig();

            if (rig == null || rig.root == null || rig.head == null || rig.leftHand == null || rig.rightHand == null)
                return;

            // Enviar a X Hz (30 por defecto)
            _sendTimer += Time.deltaTime;
            float sendInterval = 1f / Mathf.Max(1f, sendRateHz);
            if (_sendTimer < sendInterval) return;
            _sendTimer = 0f;

            var rigRoot = rig.root;
            var rigHead = rig.head;
            var rigLeft = rig.leftHand;
            var rigRight = rig.rightHand;

            // Root world
            rootPos.Value = rigRoot.position;
            rootRot.Value = rigRoot.rotation;

            // Head/hands local to rigRoot
            headPos.Value = rigRoot.InverseTransformPoint(rigHead.position);
            headRot.Value = Quaternion.Inverse(rigRoot.rotation) * rigHead.rotation;

            leftPos.Value = rigRoot.InverseTransformPoint(rigLeft.position);
            leftRot.Value = Quaternion.Inverse(rigRoot.rotation) * rigLeft.rotation;

            rightPos.Value = rigRoot.InverseTransformPoint(rigRight.position);
            rightRot.Value = Quaternion.Inverse(rigRoot.rotation) * rigRight.rotation;

            // Debug cada ~2s
            if (Time.frameCount % 120 == 0)
            {
                Debug.Log($"[OWNER SEND] root={rootPos.Value} headLocal={headPos.Value}");
            }
        }
        else
        {
            // Remotos: aplicar root world
            root.SetPositionAndRotation(
                Vector3.Lerp(root.position, rootPos.Value, lerpSpeed * Time.deltaTime),
                Quaternion.Slerp(root.rotation, rootRot.Value, lerpSpeed * Time.deltaTime)
            );

            // Aplicar head/hands en local del root
            head.SetLocalPositionAndRotation(
                Vector3.Lerp(head.localPosition, headPos.Value, lerpSpeed * Time.deltaTime),
                Quaternion.Slerp(head.localRotation, headRot.Value, lerpSpeed * Time.deltaTime)
            );

            leftHand.SetLocalPositionAndRotation(
                Vector3.Lerp(leftHand.localPosition, leftPos.Value, lerpSpeed * Time.deltaTime),
                Quaternion.Slerp(leftHand.localRotation, leftRot.Value, lerpSpeed * Time.deltaTime)
            );

            rightHand.SetLocalPositionAndRotation(
                Vector3.Lerp(rightHand.localPosition, rightPos.Value, lerpSpeed * Time.deltaTime),
                Quaternion.Slerp(rightHand.localRotation, rightRot.Value, lerpSpeed * Time.deltaTime)
            );
        }
    }
}
