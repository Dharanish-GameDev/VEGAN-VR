using UnityEngine;

[CreateAssetMenu()]
public class GameplayAudioClipsSO : AudioClipsSO 
{
	[Header("Music")]
    [Space(5)]
    public AudioClip lobbyMusic;
	public AudioClip gamePlayMusic;

	[Header("Session SFX")]
	[Space(5)]
	public AudioClip winnerTheme;
	public AudioClip loserTheme;
	public AudioClip unSafeItemCuttedFX;
	public AudioClip switchingSides;
	public AudioClip countDownSFX;
	public AudioClip tenSecondsRemaining;
	public AudioClip timeExpired;

	[Header("Effects")]
    [Space(5)]
    public AudioClip sliceSFX;
	public AudioClip cannonPopSFX;
}
