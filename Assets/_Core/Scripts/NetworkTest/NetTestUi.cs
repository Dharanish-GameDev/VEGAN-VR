using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetTestUi : MonoBehaviour
{
	public static NetTestUi instance { get; private set; }

	#region Private Variables

	[SerializeField] private Button createButton;
	[SerializeField] private Button joinButton;
	[SerializeField] private Toggle voiceToggle;

	#endregion

	#region Properties

	public Toggle VoiceToggle => voiceToggle; 

	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		instance = this;
		createButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
		joinButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
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
