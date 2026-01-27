using UnityEngine;
using Unity.Netcode.Components;
using Unity.Netcode;
public class NetworkTransformClient : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
