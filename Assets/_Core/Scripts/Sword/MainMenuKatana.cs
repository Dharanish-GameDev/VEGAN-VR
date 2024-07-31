using EzySlice;
using UnityEngine;

public class MainMenuKatana : MonoBehaviour
{
    #region Private Variables

    [Header("References")]
    [SerializeField] private Transform startSlicePoint;
    [SerializeField] private Transform endSlicePoint;
    [SerializeField] private VelocityEstimator velocityEstimator;
    [SerializeField] private SwordEffect swordEffect;
    [SerializeField] private LayerMask slicableLayerMask;
    [SerializeField] private AudioSource sliceSFX;
    [SerializeField] private MeshRenderer swordMesh;

    [Tooltip("The Material which appears on the sliced side of the mesh")]
    [SerializeField] private Material crossSectionMat;

    [Space(10)]
    [Header("Properties")]
    [SerializeField] private float cutForce;

    // Hidden
    private bool hasHit;
    Vector3 effectPosition;
    Quaternion effectRotation;
    private bool canCut;

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    private void Awake()
	{

	}
	private void Start()
	{
        ChangeCanCut(false);
	}
	private void Update()
	{
        if(canCut)
        {
            CheckForCuttableHit();
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

        if (hull != null)
        {
            // Calculate the effect position and rotation
            effectPosition = (startSlicePoint.position + endSlicePoint.position) / 2;
            effectRotation = Quaternion.LookRotation(planeNormal);

            // Trigger the sword cutting effect
            swordEffect.TriggerSwordCuttingEffect(effectPosition, effectRotation);
            sliceSFX.Play();


            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMat);
            SetUpSlicedObject(upperHull);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMat);
            SetUpSlicedObject(lowerHull);

            Destroy(upperHull, 3f);  // Destroying the generated hulls 
            Destroy(lowerHull, 3f);

            target.gameObject.SetActive(false);
            MainMenu.instance.ChangeSceneToGame();
        }
    }

    private void SetUpSlicedObject(GameObject slicedObject)
    {
        slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        slicedObject.GetComponent<Rigidbody>().AddExplosionForce(cutForce, slicedObject.transform.position, 1000);
    }
    #endregion

    #region Public Methods

    public void ChangeCanCut(bool value)
    {
        canCut = value;
        swordMesh.enabled = canCut;
    }

    #endregion
}
