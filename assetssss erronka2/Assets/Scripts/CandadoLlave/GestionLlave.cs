using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GestionLlave : MonoBehaviour
{
    [SerializeField] Image imagen;
    [SerializeField] TextMeshProUGUI errorAbrir;
    [SerializeField] GameObject key;

    [SerializeField] AbrirCandadoLlave abrirCandadoLlave;

    public InteractionLock interactionLock;

    bool picked = false;

    public void abrirLlave()
    {
        if (!picked)
        {
            StartCoroutine(MostrarError());
            interactionLock.Limpiar();
        }
        else
        {
            abrirCandadoLlave.ampliar();
        }
    }

    IEnumerator MostrarError()
    {
        errorAbrir.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        errorAbrir.gameObject.SetActive(false);
    }

    public void recogerLlave()
    {
        picked = true;
        imagen.gameObject.SetActive(true);
        key.SetActive(false);
    }

    public void llaveUsada()
    {
        imagen.gameObject.SetActive(false);
    }
}