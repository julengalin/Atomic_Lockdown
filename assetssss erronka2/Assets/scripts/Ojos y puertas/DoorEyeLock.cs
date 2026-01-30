using UnityEngine;

public class DoorEyeLock : MonoBehaviour
{
    [SerializeField] SlidingDoor door;
    [SerializeField] GameObject[] ojos;
    [SerializeField] bool abrirAlDesbloquear = true;
    [SerializeField]  bool emergenciaActiva;

    int eyesRemaining;
    bool ojosActivados;

    void Awake()
    {
        eyesRemaining = ContarOjosVivos();
        AplicarBloqueo();
    }

    int ContarOjosVivos()
    {
        if (ojos == null) return 0;

        int c = 0;
        for (int i = 0; i < ojos.Length; i++)
        {
            if (ojos[i] != null && ojos[i].activeInHierarchy)
                c++;
        }
        return c;
    }

    void AplicarBloqueo()
    {
        if (door == null) return;

        if (emergenciaActiva && eyesRemaining > 0)
            door.blocked = true;
        else
            door.blocked = false;
    }

    void ActivarOjos()
    {
        if (ojosActivados) return;
        ojosActivados = true;

        if (ojos == null) return;

        for (int i = 0; i < ojos.Length; i++)
        {
            if (ojos[i] != null)
                ojos[i].SetActive(true);
        }

        eyesRemaining = ContarOjosVivos();
    }

    public void ActivarEmergencia()
    {
        emergenciaActiva = true;
        ActivarOjos();
        AplicarBloqueo();
    }

    public void OjoMuerto()
    {
        if (eyesRemaining <= 0) return;

        eyesRemaining--;

        if (eyesRemaining <= 0)
        {
            AplicarBloqueo();

            if (abrirAlDesbloquear && door != null)
                door.ForceOpen();
        }
        else
        {
            AplicarBloqueo();
        }
    }
}
