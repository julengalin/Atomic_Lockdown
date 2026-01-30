using UnityEngine;

public class OjoHit : MonoBehaviour, IRecibeDisparo
{
    [SerializeField] DisparoTipo colorOjo = DisparoTipo.Any;
    [SerializeField] int disparosNecesarios = 6;
    [SerializeField] DoorEyeLock doorLock;
    [SerializeField] Light luz;

    int disparosRecibidos;

    public void RecibirDisparo(DisparoTipo tipo)
    {
        if (colorOjo != DisparoTipo.Any && tipo != colorOjo)
            return;

        disparosRecibidos++;

        if (disparosRecibidos >= disparosNecesarios)
        {
            if (doorLock != null)
                doorLock.OjoMuerto();

            Destroy(luz.gameObject);
            Destroy(gameObject);
        }
    }
}
