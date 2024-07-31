using EzySlice;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Katana : XRGrabInteractable
{
    #region Private Variables

    // Exposed
    [Header("References")]
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private LayerMask slicableLayerMask;
    [SerializeField] private Net_Sword netSword;

    [Tooltip("The Material which appears on the sliced side of the mesh")]
    [SerializeField] private Material crossSectionMat;

    [Space(10)]
    [Header("Properties")]
    [SerializeField] private float cutForce;

    // Hidden
    private bool hasHit;
    Vector3 effectPosition;
    Quaternion effectRotation;

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods


    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if(isSelected)
        {
            if(updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
            {
                CheckForCuttableHit();
            }
        }
    }

    #endregion

    #region Private Methods
    private void CheckForCuttableHit()
    {
        hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, slicableLayerMask);
        if (hasHit)
        {
            GameObject targetObj = hit.transform.gameObject;
            Slice(targetObj);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startSlicePoint.position, endSlicePoint.position);
    }

    private void Slice(GameObject target)
    {
        Vector3 vel = velocityEstimator.GetAccelerationEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, vel);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);
        
        if(hull != null)
        {
            // Calculate the effect position and rotation
            effectPosition = (startSlicePoint.position + endSlicePoint.position) / 2;
            effectRotation = Quaternion.LookRotation(planeNormal);

            // Trigger the sword cutting effect
            netSword.SwordEffect.TriggerSwordCuttingEffect(effectPosition, effectRotation);


            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMat);
            SetUpSlicedObject(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMat);
            SetUpSlicedObject(lowerHull);

            Destroy(upperHull,1f);  // Destroying the generated hulls 
            Destroy(lowerHull,1f);

            Slicable slicable = target.GetComponent<Slicable>();
            slicable.TurnOffColliderLocally();
            AudioSource audioSource = AudioSourceRef.Instance.AvailableDynamicSource;
            if (audioSource != null)
            {
                SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.sliceSFX, audioSource, slicable.transform.position, 0.5f);
            }
            netSword.DisableSlicable(slicable.NetworkObjectId, slicable.IsSafe);
        }
    }

    private void SetUpSlicedObject(GameObject slicedObject)
    {
        slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        slicedObject.GetComponent<Rigidbody>().AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCanPlayBoolToFalseServerRpc()
    {
        
        GameflowManager.Instance.ChangeCanPlayBooleanClientRpc(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeGameStateServerRpc()
    {
        GameflowManager.Instance.ChangeGameState();
    }
    #endregion

    #region Public Methods

    #endregion
}
