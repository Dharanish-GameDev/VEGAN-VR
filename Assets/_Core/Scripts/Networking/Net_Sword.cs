using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(XRGrabInteractable))]
public class Net_Sword : NetworkBehaviour
{
    public void DestroySlicableObject(Slicable obj)
    {
        RequestDestroyServerRpc(obj.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyServerRpc(NetworkObjectReference networkObjectReference)
    {
        if (!IsServer) return;
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            DestroyNetworkObject(networkObject.GetComponent<Slicable>());
        }
    }

    private void DestroyNetworkObject(Slicable slicable)
    {
        if (IsServer)
        {
            // Notify clients that this object is being destroyed
            DestroyClientRpc(slicable.NetworkObject);

            // Destroy the object on the server
            Destroy(slicable.gameObject);
        }
    }

    [ClientRpc]
    private void DestroyClientRpc(NetworkObjectReference networkObjectReference)
    {
        if (!IsServer)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                // Destroy the object on the client
                Destroy(networkObject.gameObject);
            }
        }
    }

}
