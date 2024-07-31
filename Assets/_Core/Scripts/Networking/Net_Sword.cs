using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using VeganVR.UI;


[RequireComponent(typeof(XRGrabInteractable))]
public class Net_Sword : NetworkBehaviour
{
    [SerializeField] private SwordEffect swordEffect;

    public SwordEffect SwordEffect => swordEffect;
    public Vector3 katanaInitialPos;
    public Quaternion katanaInitialRot;

    private void Start()
    {
       katanaInitialPos = transform.position;
       katanaInitialRot = transform.rotation;
    }

    //[ServerRpc(RequireOwnership = false)]
    //private void DisableSlicableServerRpc(ulong networkObjectId)
    //{
    //    NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    //    if (netObject != null)
    //    {
    //        Slicable slicable = netObject.GetComponent<Slicable>();
    //        if (slicable != null)
    //        {
    //            print("Disabled the Slicable by Server from the Client Request");
    //            NetworkUI.Instance.ScoreCounter.AddScore(1);
    //            slicable.TurnOffSlicable();
    //            DisableSlicableClientRpc(networkObjectId);
    //        }
    //    }
    //}
    //[ClientRpc]
    //private void DisableSlicableClientRpc(ulong networkObjectId)
    //{
    //    NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
    //    if (netObject != null)
    //    {
    //        Slicable slicable = netObject.GetComponent<Slicable>();
    //        if (slicable != null)
    //        {
    //            slicable.TurnOffSlicable();
    //        }
    //    }
    //}
    public void DisableSlicable(ulong networkObjectId,bool canAddScore)
    {
        if (IsServer)
        {
            // If this instance is the server, disable the slicable directly
            NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (netObject != null)
            {
                Slicable slicable = netObject.GetComponent<Slicable>();
                if (slicable != null)
                {
                    //slicable.TurnOffSlicable();
                    //DisableSlicableClientRpc(networkObjectId);
                    slicable.TurnoffSlicableClientRpc();
                    if (canAddScore)
                    {
                        NetworkUI.Instance.ScoreCounter.AddScoreServerRpc(0);
                    }
                    else  // It means he Cutted Chicken
                    {
                        ChickenCutted();
                    }
                    
                }
            }
        }
        else if (IsClient)
        {
            TurnOffSlicableServerRpc(networkObjectId);
            
            if(canAddScore)
            {
                NetworkUI.Instance.ScoreCounter.AddScoreServerRpc(1);
            }
            else // It means he Cutted Chicken
            {
                ChickenCutted();
            }
            
        }
    }

    [ServerRpc]
    private void TurnOffSlicableServerRpc(ulong networkObjectId)
    {
        NetworkObject netObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (netObject != null)
        {
            Slicable slicable = netObject.GetComponent<Slicable>();
            slicable.TurnoffSlicableClientRpc();
        }
    }
    public void KatanaTranformToInitial()
    {
        transform.position = katanaInitialPos;
        transform.rotation = katanaInitialRot;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCanPlayBoolToFalseServerRpc()
    {
        GameflowManager.Instance.ChangeCanPlayBooleanClientRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeGameStateServerRpc()
    {
        GameflowManager.Instance.PlaySwitcingSidesSFXClientRpc();
        GameflowManager.Instance.ChangeGameState();
    }

    private void ChickenCutted()
    {
        GameflowManager.Instance.TimerManager.StopTimerServerRpc();
        ChangeCanPlayBoolToFalseServerRpc();
        ShowNonVeganCanvasServerRpc(GameflowManager.Instance.PlayerSideManager.AttackingPlayerId);
        Invoke(nameof(ChangeGameStateServerRpc),9);
    }


    [ClientRpc]
    private void ShowNonVeganCanvasClientRpc(int refId)
    {
        NetworkUI.Instance.WinnerUI.ShowNonVeganUI(refId);
        SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.unSafeItemCuttedFX, AudioSourceRef.Instance.MiddleFieldSrc, 0.035f);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowNonVeganCanvasServerRpc(int refId)
    {
        ShowNonVeganCanvasClientRpc(refId);
    }
}
