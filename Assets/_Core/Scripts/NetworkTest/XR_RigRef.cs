using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XR_RigRef : MonoBehaviour
{
	public static XR_RigRef instance { get; private set; }

	#region Private Variables

	[SerializeField] private Transform rootTransform;
	[SerializeField] private Transform headTransform;
	[SerializeField] private Transform leftHandTransform;
	[SerializeField] private Transform rightHandTransform;

	#endregion

	#region Properties

	public Transform RootTransform => rootTransform;
	public Transform HeadTransform => headTransform;
	public Transform LeftHandTransform => leftHandTransform;
	public Transform RightHandTransform => rightHandTransform;

	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		instance = this;
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


	#endregion
}
