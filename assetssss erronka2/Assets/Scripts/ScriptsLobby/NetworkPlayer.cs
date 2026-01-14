using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [Header("Hide local meshes")]
    public Renderer[] meshToDisable;

    private NetworkVariable<Vector3> headPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> headRot = new(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> leftPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> leftRot = new(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<Vector3> rightPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> rightRot = new(writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            foreach (var r in meshToDisable)
                r.enabled = false;
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            if (VRRigReferences.Singleton == null) return;

            root.SetPositionAndRotation(
                VRRigReferences.Singleton.root.position,
                VRRigReferences.Singleton.root.rotation
            );

            headPos.Value = root.InverseTransformPoint(VRRigReferences.Singleton.head.position);
            headRot.Value = Quaternion.Inverse(root.rotation) * VRRigReferences.Singleton.head.rotation;

            leftPos.Value = root.InverseTransformPoint(VRRigReferences.Singleton.leftHand.position);
            leftRot.Value = Quaternion.Inverse(root.rotation) * VRRigReferences.Singleton.leftHand.rotation;

            rightPos.Value = root.InverseTransformPoint(VRRigReferences.Singleton.rightHand.position);
            rightRot.Value = Quaternion.Inverse(root.rotation) * VRRigReferences.Singleton.rightHand.rotation;
        }
        else
        {
            head.SetLocalPositionAndRotation(
                Vector3.Lerp(head.localPosition, headPos.Value, 20f * Time.deltaTime),
                Quaternion.Slerp(head.localRotation, headRot.Value, 20f * Time.deltaTime)
            );

            leftHand.SetLocalPositionAndRotation(
                Vector3.Lerp(leftHand.localPosition, leftPos.Value, 20f * Time.deltaTime),
                Quaternion.Slerp(leftHand.localRotation, leftRot.Value, 20f * Time.deltaTime)
            );

            rightHand.SetLocalPositionAndRotation(
                Vector3.Lerp(rightHand.localPosition, rightPos.Value, 20f * Time.deltaTime),
                Quaternion.Slerp(rightHand.localRotation, rightRot.Value, 20f * Time.deltaTime)
            );
        }
    }
}