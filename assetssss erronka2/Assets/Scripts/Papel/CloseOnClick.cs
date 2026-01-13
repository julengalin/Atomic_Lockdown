using System;
using UnityEngine;

public class CloseOnClick : MonoBehaviour
{
    [SerializeField] private CambiarCanvasPapel PruebaInteracción;
    void OnMouseDown()
    {
        PruebaInteracción.ToggleCanvas();

    }

}
