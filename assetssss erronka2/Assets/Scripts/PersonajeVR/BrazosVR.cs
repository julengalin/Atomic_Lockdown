using UnityEngine;

public class BrazosVR : MonoBehaviour
{
    [System.Serializable]
    public class Arm
    {
        public Transform shoulder;   // mixamorig:LeftShoulder / RightShoulder (opcional pero recomendable)
        public Transform upperArm;   // mixamorig:LeftArm / RightArm
        public Transform foreArm;    // mixamorig:LeftForeArm / RightForeArm
        public Transform hand;       // mixamorig:LeftHand / RightHand

        public Transform target;     // Left Controller / Right Controller (un transform “grip” si tienes)
        public Transform pole;       // LeftElbowHint / RightElbowHint

        [Range(0f, 1f)] public float weight = 1f;
    }

    public Arm leftArm;
    public Arm rightArm;

    void LateUpdate()
    {
        SolveArm(leftArm);
        SolveArm(rightArm);
    }

    void SolveArm(Arm a)
    {
        if (a.upperArm == null || a.foreArm == null || a.hand == null || a.target == null || a.pole == null) return;
        if (a.weight <= 0f) return;

        Vector3 shoulderPos = a.upperArm.position;
        Vector3 elbowPos = a.foreArm.position;
        Vector3 wristPos = a.hand.position;

        Vector3 targetPos = a.target.position;

        float upperLen = Vector3.Distance(shoulderPos, elbowPos);
        float foreLen = Vector3.Distance(elbowPos, wristPos);

        // Dirección base hacia el target
        Vector3 dirToTarget = (targetPos - shoulderPos);
        float distToTarget = dirToTarget.magnitude;

        if (distToTarget < 0.0001f) return;

        dirToTarget /= distToTarget;

        // Limitar alcance
        float maxReach = upperLen + foreLen - 0.0001f;
        float minReach = Mathf.Abs(upperLen - foreLen) + 0.0001f;
        distToTarget = Mathf.Clamp(distToTarget, minReach, maxReach);

        // Plano de la articulación (usa el pole como guía)
        Vector3 poleDir = (a.pole.position - shoulderPos).normalized;
        Vector3 planeNormal = Vector3.Cross(dirToTarget, poleDir);
        if (planeNormal.sqrMagnitude < 1e-6f)
            planeNormal = Vector3.up; // fallback
        planeNormal.Normalize();

        Vector3 planeBinormal = Vector3.Cross(planeNormal, dirToTarget).normalized;

        // Ley de cosenos para ángulo del hombro
        float cosAngle0 = (upperLen * upperLen + distToTarget * distToTarget - foreLen * foreLen) / (2f * upperLen * distToTarget);
        cosAngle0 = Mathf.Clamp(cosAngle0, -1f, 1f);
        float angle0 = Mathf.Acos(cosAngle0);

        // Posición del codo en el plano
        Vector3 elbowTarget =
            shoulderPos +
            dirToTarget * (Mathf.Cos(angle0) * upperLen) +
            planeBinormal * (Mathf.Sin(angle0) * upperLen);

        // Rotaciones: upperArm apunta al codo, foreArm al wrist/target
        Quaternion upperRot = Quaternion.LookRotation(elbowTarget - shoulderPos, planeNormal);
        Quaternion foreRot = Quaternion.LookRotation(targetPos - elbowTarget, planeNormal);

        // Mezcla por weight
        a.upperArm.rotation = Quaternion.Slerp(a.upperArm.rotation, upperRot, a.weight);
        a.foreArm.rotation = Quaternion.Slerp(a.foreArm.rotation, foreRot, a.weight);

        // La mano sigue la rotación del controlador
        a.hand.rotation = Quaternion.Slerp(a.hand.rotation, a.target.rotation, a.weight);
        a.hand.position = Vector3.Lerp(a.hand.position, targetPos, a.weight);
    }
}
