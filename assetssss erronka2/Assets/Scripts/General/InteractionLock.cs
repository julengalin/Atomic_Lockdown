using UnityEngine;

public class InteractionLock : MonoBehaviour
{
    public static InteractionLock instance;

    bool ocupado = false;
    GameObject owner;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool EstaLibre()
    {
        return !ocupado;
    }

    public bool PuedeInteractuar(GameObject obj)
    {
        return !ocupado || owner == obj;
    }

    public void Ocupar(GameObject obj)
    {
        ocupado = true;
        owner = obj;
    }

    public void Liberar(GameObject obj)
    {
        if (owner != obj) return;

        ocupado = false;
        owner = null;
    }
}
