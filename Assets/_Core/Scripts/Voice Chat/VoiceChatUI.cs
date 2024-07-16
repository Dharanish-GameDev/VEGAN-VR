using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;
using VeganVR.UI;

namespace VeganVR.VoiceChat
{
    public class VoiceChatUI : MonoBehaviour
    {
        #region Private Variables

        [Header("Voice Chat")]
        [SerializeField] private Button voiceChatToggleButton;
        [SerializeField] private GameObject vCEnableIcon;
        [SerializeField] private GameObject vCDisableIcon;
        [SerializeField] private TMP_Dropdown micSelectionDropDown;

        private bool isVoiceChatEnabled = true;
        #endregion

        #region Properties



        #endregion

        #region LifeCycle Methods

        private void Awake()
        {
            voiceChatToggleButton.onClick.AddListener(() => ToggleVoiceChat());
            micSelectionDropDown.gameObject.SetActive(false);
        }
        private void Start()
        {
            VivoxPlayer.Instance.OnVivoxInitiated += UpdateMicInputDropdown;
        }

        #endregion

        #region Private Methods

        private void DropDownValueChanged(TMP_Dropdown dropdown)
        {
            VivoxPlayer.Instance.SetActiveInputDevice(dropdown.value);
        }
        private void UpdateMicInputDropdown()
        {
            micSelectionDropDown.gameObject.SetActive(true);
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
        }
        private void EnableAndDisableVcIcon(bool value)
        {
            if (value)
            {
                vCEnableIcon.SetActive(true);
                vCDisableIcon.SetActive(false);
                Debug.Log("VC Enabled");
            }
            else
            {
                vCEnableIcon.SetActive(false);
                vCDisableIcon.SetActive(true);
                Debug.Log("VC Disabled");
            }
        }
        private void ToggleVoiceChat()
        {
            isVoiceChatEnabled = !isVoiceChatEnabled;
            EnableAndDisableVcIcon(isVoiceChatEnabled);
            NetworkUI.Instance.EnableAndDisableVoiceChat(isVoiceChatEnabled);
        }
        #endregion

        #region Public Methods


        #endregion
    }
}

