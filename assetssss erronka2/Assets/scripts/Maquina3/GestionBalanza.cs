using UnityEngine;

public class GestionBalanza : MonoBehaviour
{
    float weightL;
    float weightR;

    [SerializeField] Transform balanceRider;
    [SerializeField] float riderXNeutral = 0.00926f;
    [SerializeField] float riderXLeftMax = 0.05128f;
    [SerializeField] float riderXRightMax = -0.03545f;

    [SerializeField] Transform middleMove;
    [SerializeField] float middleXNeutral = 270f;
    [SerializeField] float middleXLeftMax = 277.358f;
    [SerializeField] float middleXRightMax = 262.642f;

    [SerializeField] Transform leftHolder;
    [SerializeField] Transform rightHolder;

    [SerializeField] float holderYNeutral = -0.01151549f;
    [SerializeField] float holderYLeftMax_LeftHolder = -0.02059f;
    [SerializeField] float holderYLeftMax_RightHolder = -0.0028f;
    [SerializeField] float holderYRightMax_LeftHolder = -0.0028f;
    [SerializeField] float holderYRightMax_RightHolder = -0.02059f;

    [SerializeField] float smooth = 6f;

    [SerializeField] Animator animator;
    [SerializeField] string animAbrir = "Abrir";
    [SerializeField] float balanceTolerance = 0.01f;

    float riderXCurrent;
    float middleXCurrent;
    float leftHolderYCurrent;
    float rightHolderYCurrent;

    bool abierto;

    void Awake()
    {
        riderXCurrent = riderXNeutral;
        middleXCurrent = middleXNeutral;
        leftHolderYCurrent = holderYNeutral;
        rightHolderYCurrent = holderYNeutral;

        AplicarRider(riderXNeutral);
        AplicarMiddleMove(middleXNeutral);
        AplicarHoldersY(holderYNeutral, holderYNeutral);
    }

    void Update()
    {
        ActualizarVisual();
        ComprobarBalance();
    }

    public void EntrarPeso(float peso, bool left)
    {
        if (left) weightL += peso;
        else weightR += peso;
    }

    public void SalirPeso(float peso, bool left)
    {
        if (left) weightL -= peso;
        else weightR -= peso;
    }

    void ActualizarVisual()
    {
        float sum = Mathf.Abs(weightL) + Mathf.Abs(weightR);

        float targetRiderX = riderXNeutral;
        float targetMiddleX = middleXNeutral;
        float targetLeftHolderY = holderYNeutral;
        float targetRightHolderY = holderYNeutral;

        if (sum > 0.0001f)
        {
            float ratio = (weightL - weightR) / sum;
            ratio = Mathf.Clamp(ratio, -1f, 1f);

            if (ratio >= 0f)
            {
                targetRiderX = Mathf.Lerp(riderXNeutral, riderXLeftMax, ratio);
                targetMiddleX = Mathf.Lerp(middleXNeutral, middleXLeftMax, ratio);
                targetLeftHolderY = Mathf.Lerp(holderYNeutral, holderYLeftMax_LeftHolder, ratio);
                targetRightHolderY = Mathf.Lerp(holderYNeutral, holderYLeftMax_RightHolder, ratio);
            }
            else
            {
                float t = -ratio;
                targetRiderX = Mathf.Lerp(riderXNeutral, riderXRightMax, t);
                targetMiddleX = Mathf.Lerp(middleXNeutral, middleXRightMax, t);
                targetLeftHolderY = Mathf.Lerp(holderYNeutral, holderYRightMax_LeftHolder, t);
                targetRightHolderY = Mathf.Lerp(holderYNeutral, holderYRightMax_RightHolder, t);
            }
        }

        riderXCurrent = Mathf.Lerp(riderXCurrent, targetRiderX, Time.deltaTime * smooth);
        middleXCurrent = Mathf.LerpAngle(middleXCurrent, targetMiddleX, Time.deltaTime * smooth);
        leftHolderYCurrent = Mathf.Lerp(leftHolderYCurrent, targetLeftHolderY, Time.deltaTime * smooth);
        rightHolderYCurrent = Mathf.Lerp(rightHolderYCurrent, targetRightHolderY, Time.deltaTime * smooth);

        AplicarRider(riderXCurrent);
        AplicarMiddleMove(middleXCurrent);
        AplicarHoldersY(leftHolderYCurrent, rightHolderYCurrent);
    }

    void ComprobarBalance()
    {
        if (abierto) return;

        if (Mathf.Abs(weightL - weightR) <= balanceTolerance && weightL + weightR != 0)
        {
            if (animator != null)
                animator.Play(animAbrir);

            abierto = true;
        }
    }

    void AplicarRider(float x)
    {
        if (balanceRider == null) return;

        Vector3 lp = balanceRider.localPosition;
        lp.x = x;
        balanceRider.localPosition = lp;
    }

    void AplicarMiddleMove(float rotX)
    {
        if (middleMove == null) return;

        middleMove.rotation = Quaternion.Euler(rotX, 90f, -90f);
    }

    void AplicarHoldersY(float leftY, float rightY)
    {
        if (leftHolder != null)
        {
            Vector3 lp = leftHolder.localPosition;
            lp.y = leftY;
            leftHolder.localPosition = lp;
        }

        if (rightHolder != null)
        {
            Vector3 lp = rightHolder.localPosition;
            lp.y = rightY;
            rightHolder.localPosition = lp;
        }
    }
}
