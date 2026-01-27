using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public ElevatorController elevator;
    bool playerInside;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pulsada dentro del botón");
            elevator.ToggleElevator();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player dentro del botón");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("Player fuera del botón");
        }
    }

    void OnGUI()
    {
        if (playerInside)
            GUI.Label(new Rect(20, 20, 400, 30), "Klikatu E igogailua erabiltzeko");
    }
}
