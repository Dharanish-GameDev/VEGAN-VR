using System.Collections.Generic;
using UnityEngine;

public class AudioSourceRef : MonoBehaviour
{
	public static AudioSourceRef Instance { get; private set; }

	#region Private Variables

	[Header("Audio_Sources")]
	[SerializeField] private AudioSource bgAudioSource;
	[SerializeField] private AudioSource middleFieldSource;
	[SerializeField] private List<AudioSource> dynamicSources;
	[SerializeField] private AudioSource cannonSource;

	#endregion

	#region Properties

	public AudioSource BG_AudioSrc => bgAudioSource;
	public AudioSource MiddleFieldSrc => middleFieldSource;
    public AudioSource AvailableDynamicSource
	{
		get
		{
			foreach (AudioSource audioSrc in dynamicSources)
			{
				if (!audioSrc.isPlaying)
				{
					return audioSrc;
				}
			}
			return null; // Return null if all audio sources are playing
		}
	}

	public AudioSource CannonSrc => cannonSource;
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
