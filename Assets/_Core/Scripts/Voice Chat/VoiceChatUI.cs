using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

namespace VeganVR.VoiceChat
{
    public class VoiceChatUI : MonoBehaviour
    {
        #region Private Variables
        [Header("Voice Input")]
        [SerializeField] private TMP_Dropdown micSelectionDropDown;

        [Header("Speech Dedection")]
        [SerializeField] private Image speechDedectorImage;
        [SerializeField] private Sprite speakingSprite;
        [SerializeField] private Sprite notSpeakingSprite;

        [SerializeField] private Image deviceEnergyMask;



        private VivoxParticipant vivoxParticipant;
        const float k_voiceMeterSpeed = 3;
        #endregion

        #region Properties



        #endregion

        #region LifeCycle Methods

        private void Awake()
        {

        }
        private void Start()
        {
            VivoxPlayer.Instance.OnVivoxInitiated += VivoxPlayer_OnVivoxInitiated;
        }
        private void Update()
        {
            //if (VivoxService.Instance.ActiveChannels.Count > 0)
            //{
            //    var channel = VivoxService.Instance.ActiveChannels.FirstOrDefault();
            //    var localParticipant = channel.Value.FirstOrDefault(p => p.IsSelf);
            //    deviceEnergyMask.fillAmount = Mathf.Lerp(deviceEnergyMask.fillAmount, (float)localParticipant.AudioEnergy*1.2f, Time.deltaTime * k_voiceMeterSpeed);
            //}
        }

        #endregion

        #region Private Methods

        private void DropDownValueChanged(TMP_Dropdown dropdown)
        {
            VivoxPlayer.Instance.SetActiveInputDevice(dropdown.value);
        }
        private void VivoxPlayer_OnVivoxInitiated()
        {
            micSelectionDropDown.options.Clear();
            List<string> MicNamesList = new List<string>();
            for (int i = 0; i < VivoxService.Instance.AvailableInputDevices.Count; i++)
            {
                if (VivoxService.Instance.AvailableInputDevices[i].DeviceName != "No Device")
                {
                    MicNamesList.Add(VivoxService.Instance.AvailableInputDevices[i].DeviceName);
                }
            }
            micSelectionDropDown.AddOptions(MicNamesList);

            VivoxPlayer.Instance.SetActiveInputDevice(micSelectionDropDown.value); // Sets 0 as the Initial Mic to Vivox

            micSelectionDropDown.onValueChanged.AddListener(delegate { DropDownValueChanged(micSelectionDropDown); });

            //VivoxService.Instance.ParticipantAddedToChannel += Vivox_ParticipantAddedToChannel;
            //deviceEnergyMask.fillAmount = 0;
        }

        private void Vivox_ParticipantAddedToChannel(VivoxParticipant participant)
        {
            vivoxParticipant = participant;
            vivoxParticipant.ParticipantSpeechDetected += ChangeSpeechDedectorSprite;
            vivoxParticipant.ParticipantMuteStateChanged += ChangeSpeechDedectorSprite;
        }


        private void SpeechDedected()
        {
            Debug.Log("Speech Dedected");
        }

        private void MuteStateChanged()
        {
            Debug.Log("Mute State Changed");
        }

        private void ChangeSpeechDedectorSprite()
        {
            if (vivoxParticipant.SpeechDetected)
            {
                speechDedectorImage.sprite = speakingSprite;
                speechDedectorImage.gameObject.transform.localScale = Vector3.one;
            }
            else
            {
                speechDedectorImage.sprite = notSpeakingSprite;
            }
        }
        #endregion

        #region Public Methods


        #endregion
    }
}

