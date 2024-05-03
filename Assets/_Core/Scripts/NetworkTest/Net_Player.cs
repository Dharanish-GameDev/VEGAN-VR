using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Net_Player : NetworkBehaviour
{
    #region Private Variables

    [SerializeField] private Transform rootTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    [SerializeField] private List<Renderer> disableMeshList = new();

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DisableMesh();
    }
    private void Awake()
	{

	}
	private void Start()
	{

	}
	private void Update()
    {
        SyncPositionAndRotation();
    }



    #endregion

    #region Private Methods

    private void SyncPositionAndRotation()
    {
        if (!IsOwner) return;

        rootTransform.position = XR_RigRef.instance.RootTransform.position;
        rootTransform.rotation = XR_RigRef.instance.RootTransform.rotation;

        headTransform.position = XR_RigRef.instance.HeadTransform.position;
        headTransform.rotation = XR_RigRef.instance.HeadTransform.rotation;

        leftHandTransform.position = XR_RigRef.instance.LeftHandTransform.position;
        leftHandTransform.rotation = XR_RigRef.instance.LeftHandTransform.rotation;

        rightHandTransform.position = XR_RigRef.instance.RightHandTransform.position;
        rightHandTransform.rotation = XR_RigRef.instance.RightHandTransform.rotation;
    }

    private void DisableMesh()
    {
        if (!IsOwner) return;
        foreach (Renderer mesh in disableMeshList)
        {
            mesh.enabled = false;
        }
    }

    #endregion

    #region Public Methods


    #endregion
}
