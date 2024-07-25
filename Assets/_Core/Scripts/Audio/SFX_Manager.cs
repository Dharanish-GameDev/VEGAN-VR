using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Manager : MonoBehaviour
{
    public static SFX_Manager instance { get; private set; }

    #region Private Variables

    [SerializeField] private GameplayAudioClipsSO gameplayAudioClips;
    [SerializeField] private UI_SoundEffectsSO uiAudioClips;

    #endregion

    #region Properties
    public GameplayAudioClipsSO GamePlayAudioClips => gameplayAudioClips;
    public UI_SoundEffectsSO UIAudioClips => uiAudioClips;

    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(this.gameObject);
    }

    #region Public Methods

    public void PlayOneShot(AudioClip clip, AudioSource source, float volume)
    {
        source.Stop();
        source.volume = volume;
        source.PlayOneShot(clip);
    }
    public void PlayOneShot(AudioClip clip, AudioSource source,Vector3 audioSrcPos, float volume)
    {
        source.volume = volume;
        source.transform.position = audioSrcPos;
        source.PlayOneShot(clip);
    }

    public void ChangeAudioSourcePos(AudioSource source, Vector3 audioSrcPos)
    {
        source.transform.position = audioSrcPos;
    }
    public void ChangeClipsOnLoopingAudioSrc(AudioClip clip,AudioSource audioSource,float volume)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }
    public void SetVolume(AudioSource audioSource, float volume)
    {
        audioSource.volume = volume;
    }
    #endregion
}
