using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
public class PlayerDetailsUI : NetworkBehaviour
{

	#region Private Variables

	[SerializeField] private GameObject playerDetailsUI;
	[SerializeField] private Image voiceStateImage;
	[SerializeField] private Sprite speakingSprite;
	[SerializeField] private Sprite notSpeakingSprite;
	[SerializeField] private Sprite mutedSprite;
	[SerializeField] private TextMeshProUGUI playerNameText;

	private VivoxParticipant voiceParticipant;


    #endregion

    #region Properties

    public Transform PlayerDetailsUiObject => playerDetailsUI.transform;

    #endregion

    #region LifeCycle Methods

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
		if (IsOwner)
		{
			VivoxService.Instance.ParticipantAddedToChannel += ParticipantAddedToChannel;
		}
    }

    public override void OnDestroy()
    {
		base.OnDestroy();
        if (IsOwner && VivoxService.Instance != null)
        {
            VivoxService.Instance.ParticipantAddedToChannel -= ParticipantAddedToChannel;
        }

        if (voiceParticipant != null)
        {
            voiceParticipant.ParticipantSpeechDetected -= OnParticipantSpeechStateChanged;
            voiceParticipant.ParticipantMuteStateChanged -= OnParticipantSpeechStateChanged;
        }

    }

    private void ParticipantAddedToChannel(VivoxParticipant obj)
    {
        voiceParticipant = obj;
        voiceParticipant.ParticipantSpeechDetected += OnParticipantSpeechStateChanged;
        voiceParticipant.ParticipantMuteStateChanged += OnParticipantSpeechStateChanged;
    }


    #endregion

    #region Private Methods

    private void OnParticipantSpeechStateChanged()
    {
        if(IsOwner)
        {
            UpdateVoiceStateServerRpc(voiceParticipant.IsMuted, voiceParticipant.SpeechDetected);
        }
    }


    [ServerRpc]
    private void UpdateVoiceStateServerRpc(bool muteState, bool speechDetected)
    {
        UpdateVoiceStateClientRpc(muteState, speechDetected);
    }

    [ClientRpc]
    private void UpdateVoiceStateClientRpc(bool muteState, bool speechDetected)
    {
        UpdateVoiceStateUI(muteState, speechDetected);
    }

    private void UpdateVoiceStateUI(bool isMuted, bool isSpeaking)
    {
        if (isMuted)
        {
            voiceStateImage.sprite = mutedSprite;
        }
        else if (isSpeaking)
        {
            voiceStateImage.sprite = speakingSprite;
        }
        else
        {
            voiceStateImage.sprite = notSpeakingSprite;
        }
    }



    #endregion

    #region Public Methods
    public void ChangePlayerName()
	{
		playerNameText.text = OwnerClientId == 0 ? "BLUE PLAYER" :"RED PLAYER";
	}
   
    #endregion
}
