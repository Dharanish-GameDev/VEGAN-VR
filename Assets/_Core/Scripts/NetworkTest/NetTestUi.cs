using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetTestUi : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private Button createButton;
	[SerializeField] private Button joinButton;

	#endregion

	#region Properties



	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		createButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
		joinButton.onClick.AddListener(()=>NetworkManager.Singleton.StartClient());
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
