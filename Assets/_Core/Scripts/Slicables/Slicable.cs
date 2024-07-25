using Unity.Netcode;
using UnityEngine;

public class Slicable : NetworkBehaviour
{
    #region Private Variables

    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Collider slicablecollider;
    [SerializeField] private SlicableType scicableType;

    private CannonController cannonController;
    private SlicableStateEnum slicableState = SlicableStateEnum.Available;

    #endregion

    #region Properties

    public bool IsSafe => scicableType == SlicableType.Safe;

    public bool IsThrowableAvailable => slicableState == SlicableStateEnum.Available;

    #endregion

    #region LifeCycle Methods

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        cannonController = CannonController.Instance;
        TurnOffSlicable();
    }

    #endregion

    #region Private Methods

    private void OnCollisionEnter(Collision collision)
    {
        if(GameflowManager.Instance.SlicabbleBlockerList.Contains(collision.gameObject))
        {
            if(IsServer)
            {
                TurnoffSlicableClientRpc();
            }
        }
    }

    private void TurnOnSlicable()
    {
        meshRenderer.enabled = true;
        trailRenderer.enabled = true;
        rb.isKinematic = false;
        slicablecollider.enabled = true;
    }

    private void TransformToShotPoint()
    {
        transform.position = cannonController.ShotPointTransform.position;
        transform.rotation = cannonController.ShotPointTransform.rotation;
    }
    private void ChangeSlicableState(SlicableStateEnum state)
    {
        slicableState = state;
    }

    #endregion

    #region Public Methods
    public void ShootThisSlicable()
    {
        TransformToShotPoint();
        TurnOnSlicable();
        rb.velocity = cannonController.ShotPointTransform.up * cannonController.BlastPower;
        ChangeSlicableState(SlicableStateEnum.Travelling);
    }
    public void TurnOffSlicable()
    {
        meshRenderer.enabled = false;
        trailRenderer.enabled = false;
        slicablecollider.enabled = false;
        rb.isKinematic = true;
        ChangeSlicableState(SlicableStateEnum.Available);
    }
    public void TurnOffColliderLocally()
    {
        slicablecollider.enabled = false;
    }

    [ClientRpc]
    public void TurnoffSlicableClientRpc()
    {
        TurnOffSlicable();
    }

    [ServerRpc]
    public void TurnOffSlicableServerRpc()
    {
        TurnoffSlicableClientRpc();
    }

    
    #endregion
}

public enum SlicableStateEnum
{
    Available,
    Travelling,
}

public enum SlicableType
{
    Safe,
    UnSafe
}