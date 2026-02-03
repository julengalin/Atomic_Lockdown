using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class XRGravedad : MonoBehaviour
{
    public float gravedad = -9.81f;
    public float fuerzaPegadoSuelo = -2f;
    public float velocidadMaxCaida = -50f;

    private CharacterController cc;
    private float velY;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Si está en el suelo, mantenlo pegado
        if (cc.isGrounded && velY < 0f)
            velY = fuerzaPegadoSuelo;
        else
            velY += gravedad * Time.deltaTime;

        if (velY < velocidadMaxCaida) velY = velocidadMaxCaida;

        cc.Move(Vector3.up * velY * Time.deltaTime);
    }

    // Llama a esto desde tu script de salto
    public void Saltar(float fuerzaSalto)
    {
        if (cc.isGrounded)
            velY = Mathf.Sqrt(fuerzaSalto * -2f * gravedad); // fórmula de salto
    }
}
