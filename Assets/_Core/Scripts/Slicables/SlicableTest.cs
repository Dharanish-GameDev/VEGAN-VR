using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlicableTest : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private Rigidbody rb;
	[SerializeField] private MeshRenderer meshRenderer;

	#endregion

	#region Properties

	public Rigidbody Rb => rb;

	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		TurnOffMesh();
    }
	private void Start()
	{

	}
	private void Update()
	{

	}
	
	#endregion

	#region Private Methods


	#endregion

	#region Public Methods

	public void TurnOffMesh()
	{
		meshRenderer.enabled = false;
		rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
	}

	public void TurnOnMesh()
	{
		meshRenderer.enabled = true;
		rb.useGravity = true;
        Invoke(nameof(TurnOffMesh), 2);
    }

	public void TransformToShotPoint(Transform pointTransform)
	{
		transform.position = pointTransform.position;
		transform.rotation = pointTransform.rotation;
	}

	#endregion
}
