using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbrirCandadoLlave : MonoBehaviour
{

    public GestionLlave gestionLlave;
    public RecogerLlave recogerLlave;
    [SerializeField] Camera cam;
    public Button button;

    public GameObject key;

    public bool playMode = false;
    Vector3 posicionInicial;

    Vector3 candadoOffset = new Vector3(0.583f, -0.5f, 6.899f);
    Vector3 llaveOffset = new Vector3(-5.477f, -3.2f, 6.899f);

    private void Start()
    {
        posicionInicial = gameObject.transform.position;
    }

    private void OnMouseDown()
    {
        if (!playMode) gestionLlave.abrirLlave();
    }


    public void ampliar()
    {
        playMode = true;
        button.gameObject.SetActive(true);

        Debug.Log(playMode);

        transform.position = cam.transform.position + candadoOffset;

        key.SetActive(true);
        key.transform.position = cam.transform.position + llaveOffset;
        recogerLlave.inGame();
    }

    public void salir()
    {
        playMode=false;
        button.gameObject.SetActive(false);
        key.SetActive(false);
        if (!playMode) transform.position = posicionInicial;
        recogerLlave.exitGame();
    }

}
