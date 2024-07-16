using Unity.Netcode;
using UnityEngine;

public class SlicableTest : NetworkBehaviour
{
    #region Private Variables

    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider slicablecollider;

    private CannonController cannonController;
    private SlicableStateEnum slicableState = SlicableStateEnum.Available;

    #endregion

    #region Properties

    public SlicableStateEnum SlicableState => slicableState;

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

   

    private void TurnOnSlicable()
    {
        meshRenderer.enabled = true;
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
        slicablecollider.enabled = false;
        rb.isKinematic = true;
        ChangeSlicableState(SlicableStateEnum.Available);
    }
    public void TurnOffColliderLocally()
    {
        slicablecollider.enabled = false;
    }
    #endregion
}

public enum SlicableStateEnum
{
    Available,
    Travelling,
}