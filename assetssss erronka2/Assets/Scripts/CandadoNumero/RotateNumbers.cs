using UnityEngine;

public class RotateNumbers : MonoBehaviour
{
    [SerializeField] private JuegoCandado juegoCandado;
    [SerializeField] private int posicion;

    [SerializeField] private float initialRotationZ;
    [SerializeField] private float stepDegrees = 36f; 

    private float currentZ;

    public InteractionLock interactionLock;
    public InteractionType tipo = InteractionType.CandadoNumerico;

    private void Start()
    {
        currentZ = initialRotationZ;
        ApplyRotation();
    }

    void OnMouseDown()
    {
        if (interactionLock.tipoActual != InteractionType.None && interactionLock.tipoActual != tipo)
        {
            return;
        }
        else if (interactionLock.tipoActual == InteractionType.None)
        {
            interactionLock.Set(tipo);
        }
        currentZ -= stepDegrees;

        currentZ = (currentZ % 360f + 360f) % 360f;

        ApplyRotation();

        if (juegoCandado != null)
            juegoCandado.updatePos(posicion);


        Debug.Log("se ha clicado la rueda" + posicion);
    }

    public void ResetWheel()
    {
        currentZ = initialRotationZ;
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        transform.localRotation = Quaternion.Euler(-90f, 0f, currentZ);
    }
}
