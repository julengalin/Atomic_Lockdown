using Unity.Netcode;
using UnityEngine;

public class NetworkConnect : MonoBehaviour
{
   

    public void Create()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Lobby created");
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
