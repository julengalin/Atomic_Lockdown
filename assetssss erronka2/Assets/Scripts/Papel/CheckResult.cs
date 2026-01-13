using UnityEngine;

public class CheckResult : MonoBehaviour
{
    [SerializeField] private JuegoCandado juegoCandado;

    void OnMouseDown()
    {
        Debug.Log("comprobando resultado"); 
        juegoCandado.check();
    }
}
