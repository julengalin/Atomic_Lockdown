using UnityEngine;
using UnityEngine.InputSystem;

public class AbrirCandado : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;
    public bool playMode = false;
    [SerializeField] Camera cam;
    Vector3 posicionInicial;

    Vector3 candadoOffset = new Vector3(-0.381f, -0.923f, 2.288f);

    private void Start()
    {
        posicionInicial = gameObject.transform.position;
    }

    void OnMouseDown()
    {
        if(!playMode) ToggleState();
    }

    public void ToggleState()
    {
        playMode = !playMode;
        Debug.Log(playMode);

        GetComponent<Collider>().enabled = !playMode;

        transform.position = cam.transform.position + candadoOffset;

        if(!playMode) transform.position = posicionInicial; 
        
        if (canvasObject == null) return;

        canvasObject.SetActive(!canvasObject.activeSelf);


    }
}
