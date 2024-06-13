using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(XRGrabInteractable))]
public class Net_Sword : NetworkBehaviour
{
    public void DestroySlicableObject(Slicable obj)
    {
        DestroySliceObjectServerRpc(obj.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroySliceObjectServerRpc(NetworkObjectReference networkObjectReference)
    {
        networkObjectReference.TryGet(out NetworkObject networkObject);
        Slicable obj = networkObject.GetComponent<Slicable>();
        networkObject.Despawn();
        obj.Destroyself();
    }
}
