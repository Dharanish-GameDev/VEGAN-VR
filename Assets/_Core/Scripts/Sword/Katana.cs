using EzySlice;
using System;
using System.Collections;
using System.Collections.Generic;
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
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMat);
            SetUpSlicedObject(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMat);
            SetUpSlicedObject(lowerHull);

            Slicable slicabble = target.GetComponent<Slicable>();
            netSword.DestroySlicableObject(slicabble);
        }
    }

    private void SetUpSlicedObject(GameObject slicedObject)
    {
        slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        slicedObject.GetComponent<Rigidbody>().AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }


    #endregion

    #region Public Methods


    #endregion
}
