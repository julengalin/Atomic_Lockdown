using UnityEngine;
using UnityEngine.InputSystem;

public class CambiarCanvasPapel : MonoBehaviour
{
    [SerializeField] private GameObject canvasObject;

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleCanvas();
        }
    }

    public void ToggleCanvas()
    {
        if (canvasObject == null) return;

        canvasObject.SetActive(!canvasObject.activeSelf);
    }
}
