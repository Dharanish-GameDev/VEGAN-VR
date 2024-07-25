using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceRef : MonoBehaviour
{
	public static AudioSourceRef Instance { get; private set; }

	#region Private Variables

	[Header("Audio_Sources")]
	[SerializeField] private AudioSource bgAudioSource;
	[SerializeField] private AudioSource middleFieldSource;


	[Space(10)]
	[Header("Transform_Refs")]
	[SerializeField] private Transform  cannonTransform;

	#endregion

	#region Properties

	public AudioSource BG_AudioSrc => bgAudioSource;
	public AudioSource MiddleFieldSrc => middleFieldSource;

	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		Instance = this;
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
