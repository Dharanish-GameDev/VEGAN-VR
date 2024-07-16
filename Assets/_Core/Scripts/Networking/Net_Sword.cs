using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(XRGrabInteractable))]
public class Net_Sword : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    private void DisableSlicableServerRpc(ulong networkObjectId)
    {
        NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (netObject != null)
        {
            SlicableTest slicable = netObject.GetComponent<SlicableTest>();
            if (slicable != null)
            {
                print("Disabled the Slicable by Server from the Client Request");
                slicable.TurnOffSlicable();
                DisableSlicableClientRpc(networkObjectId);
            }
        }
    }

    [ClientRpc]
    private void DisableSlicableClientRpc(ulong networkObjectId)
    {
        NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (netObject != null)
        {
            SlicableTest slicable = netObject.GetComponent<SlicableTest>();
            if (slicable != null)
            {
                print("Disabled the Slicable by Server from the Server Request");
                slicable.TurnOffSlicable();
            }
        }
    }

    public void DisableSlicable(ulong networkObjectId)
    {
        if (IsServer)
        {
            // If this instance is the server, disable the slicable directly
            NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (netObject != null)
            {
                SlicableTest slicable = netObject.GetComponent<SlicableTest>();
                if (slicable != null)
                {
                    slicable.TurnOffSlicable();
                    DisableSlicableClientRpc(networkObjectId);
                }
            }
        }
        else if (IsClient)
        {
            // If this instance is a client, request the server to disable the slicable
            DisableSlicableServerRpc(networkObjectId);
        }
    }
}
