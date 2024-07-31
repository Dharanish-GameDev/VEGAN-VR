using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VeganVR.Player.Local;

public class NetPlayerGrab : NetworkBehaviour
{
    #region Private Variables

    [SerializeField] private List<UnityEngine.GameObject> handsList = new List<UnityEngine.GameObject>();

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
        interactor.selectEntered.AddListener(OnSelectEnteringGrab);
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
        interactor.selectEntered.RemoveListener(OnSelectEnteringGrab);
        interactor.selectExited.RemoveListener(OnSelectExitingGrab);
    }
    private void OnSelectEnteringGrab(SelectEnterEventArgs args)
    {
        if (IsClient && IsOwner)
        {
            CheckAndDisableHandOnNet(args);
            ;
            if (args.interactableObject.transform.TryGetComponent<NetworkObject>(out NetworkObject networkObjectSelected))
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
            CheckAndEnableHandOnNet(args);

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

    /// <summary>
    /// 0 - LeftHand , 1 - RightHand
    /// </summary>
    /// <param name="handIndex"></param>
    [ServerRpc]
    private void DisableGrabbingHandServerRpc(int handIndex)
    {
        DisableGrabbingHandClientRpc(handIndex);
    }

    [ClientRpc]
    private void DisableGrabbingHandClientRpc(int handIndex)
    {
        if (handIndex > 2) return;
        handsList[handIndex].SetActive(false);
    }

    /// <summary>
    /// 0 - Left Hand , 1 - RightHand
    /// </summary>
    /// <param name="handIndex"></param>
    [ServerRpc]
    private void EnableGrabbingHandServerRpc(int handIndex)
    {
        EnableGrabbingHandClientRpc(handIndex);
    }

    [ClientRpc]
    private void EnableGrabbingHandClientRpc(int handIndex)
    {
        if (handIndex > 2) return;
        handsList[(handIndex)].SetActive(true);
    }

    private void CheckAndDisableHandOnNet(SelectEnterEventArgs args)
    {
        if (args.interactorObject.Equals(XR_RigRef.instance.LeftDirectInteractor))
        {
            DisableGrabbingHandServerRpc(0);
        }
        if (args.interactorObject.Equals(XR_RigRef.instance.RightDirectInteractor))
        {
            DisableGrabbingHandServerRpc(1);
        }
    }

    private void CheckAndEnableHandOnNet(SelectExitEventArgs args)
    {
        if (args.interactorObject.Equals(XR_RigRef.instance.LeftDirectInteractor))
        {
            EnableGrabbingHandServerRpc(0);
        }
        if (args.interactorObject.Equals(XR_RigRef.instance.RightDirectInteractor))
        {
            EnableGrabbingHandServerRpc(1);
        }
    }

    #endregion

    #region Public Methods


    #endregion
}