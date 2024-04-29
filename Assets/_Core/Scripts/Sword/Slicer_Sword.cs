using EzySlice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer_Sword : MonoBehaviour
{
	#region Private Variables

    // Exposed
	[Header("References")]
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private LayerMask slicableLayerMask;

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

    private void Awake()
	{

	}
	private void Start()
	{

	}
	private void FixedUpdate()
	{
        hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, slicableLayerMask);
        if (hasHit)
        {
            GameObject targetObj = hit.transform.gameObject;
            Slice(targetObj);
        }
    }

    #endregion

    #region Private Methods

    private void Slice(GameObject target)
    {
        Vector3 vel = velocityEstimator.GetAccelerationEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, vel);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);
        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMat);
            SetUpSlicedObject(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMat);
            SetUpSlicedObject(lowerHull);

            Destroy(target);
        }
    }
    private void SetUpSlicedObject(GameObject slicedObject)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);
    }

    #endregion

    #region Public Methods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(startSlicePoint.position, endSlicePoint.position);
    }

    #endregion
}
