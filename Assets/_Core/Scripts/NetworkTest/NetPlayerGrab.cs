using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetPlayerGrab : NetworkBehaviour
{
    #region Private Variables


    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SubscribeGrabForInteractor();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        UnSubscribeGrabForInteractor();
    }

    #endregion

    #region Private Methods

    private void SubscribeGrabForInteractor()
    {
        SubscribeGrab(XR_RigRef.instance.RightDirectInteractor);
        SubscribeGrab(XR_RigRef.instance.LeftDirectInteractor);
        SubscribeGrab(XR_RigRef.instance.RightRayInteractor);
        SubscribeGrab(XR_RigRef.instance.LeftRayInteractor);
    }
    private void SubscribeGrab(XRBaseInteractor interactor)
    {
        interactor.selectEntered.AddListener(OnSelectGrabbable);
        interactor.selectExited.AddListener(OnSelectExitingGrab);
    }
    private void UnSubscribeGrabForInteractor()
    {
        UnSubscribeGrab(XR_RigRef.instance.RightDirectInteractor);
        UnSubscribeGrab(XR_RigRef.instance.LeftDirectInteractor);
        UnSubscribeGrab(XR_RigRef.instance.RightRayInteractor);
        UnSubscribeGrab(XR_RigRef.instance.LeftRayInteractor);
    }
    private void UnSubscribeGrab(XRBaseInteractor interactor)
    {
        interactor.selectEntered.RemoveListener(OnSelectGrabbable);
        interactor.selectExited.RemoveListener(OnSelectExitingGrab);
    }
    private void OnSelectGrabbable(SelectEnterEventArgs args)
    {
        if (IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = args.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null)
            {
                RequestGrabbaleOwnershipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    [ServerRpc]
    private void RequestGrabbaleOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.ChangeOwnership(newOwnerClientId);
        }
        else
        {
            Debug.LogError($"Cannot transfer Ownership to the Client {newOwnerClientId}");
        }
    }
    private void OnSelectExitingGrab(SelectExitEventArgs args)
    {
        if (IsClient && IsOwner)
        {
            NetworkObject networkObjectSelected = args.interactableObject.transform.GetComponent<NetworkObject>();
            if (networkObjectSelected != null)
            {
                RemoveGrabbaleOwnershipServerRpc(OwnerClientId, networkObjectSelected);
            }
        }
    }

    [ServerRpc]
    private void RemoveGrabbaleOwnershipServerRpc(ulong newOwnerClientId, NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            networkObject.RemoveOwnership();
        }
        else
        {
            Debug.LogError($"Cannot remove Ownership from the Client {newOwnerClientId}");
        }
    }

    #endregion

    #region Public Methods


    #endregion
}
