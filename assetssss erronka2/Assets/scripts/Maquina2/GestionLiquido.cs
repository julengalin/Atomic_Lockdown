using UnityEngine;

public class GestionLiquido : MonoBehaviour
{
    Renderer rend;
    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    float wobbleAmountX;
    float wobbleAmountZ;
    float wobbleAmountToAddX;
    float wobbleAmountToAddZ;
    float pulse;
    float time = 0.5f;

    [SerializeField] float fillMin = 0.3f;
    [SerializeField] float fillMax = 0.8f;
    [SerializeField] float anguloMin = 0f;
    [SerializeField] float anguloMax = 120f;
    [SerializeField] float fill = 0.3f;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        time += Time.deltaTime;
        wobbleAmountToAddX = Mathf.Lerp(wobbleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wobbleAmountToAddZ = Mathf.Lerp(wobbleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        pulse = 2 * Mathf.PI * WobbleSpeed;
        wobbleAmountX = wobbleAmountToAddX * Mathf.Sin(pulse * time);
        wobbleAmountZ = wobbleAmountToAddZ * Mathf.Sin(pulse * time);

        rend.material.SetFloat("_WobbleX", wobbleAmountX);
        rend.material.SetFloat("_WobbleZ", wobbleAmountZ);

        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;

        wobbleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        wobbleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }

    public void cambiarFill(float angulo)
    {
        if (rend == null) return;

        float t = Mathf.InverseLerp(anguloMin, anguloMax, Mathf.Clamp(angulo, anguloMin, anguloMax));
        fill = Mathf.Lerp(fillMin, fillMax, t);
        rend.material.SetFloat("_Fill", fill);
    }
}
