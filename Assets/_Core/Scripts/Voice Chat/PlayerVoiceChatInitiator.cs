using Unity.Netcode;
using Unity.Services.Vivox;
using UnityEngine;
using VeganVR.UI;

namespace VeganVR.VoiceChat
{
    public class PlayerVoiceChatInitiator : NetworkBehaviour
    {

        #region Private Variables

        string displayName;
        private VivoxParticipant voiceChatParticipant;
        private PlayerDetailsUI playerDetailsUI;

        #endregion

        #region Properties

        public VivoxParticipant VoiceChatParticipant => voiceChatParticipant;
        #endregion

        #region LifeCycle Methods

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            if (NetworkUI.Instance.VoiceToggle.isOn)
            {
                VivoxPlayer.Instance.LoginToVivoxAsync();
            }
            else
            {
                Debug.Log("Player Decided not to Join in Voice Chat");
            }
            NetworkUI.Instance.VoiceToggle.onValueChanged.AddListener(VoiceToggleValueChanged);
            playerDetailsUI = GetComponent<PlayerDetailsUI>();
        }


        
        #endregion

        #region Private Methods

        private void VoiceToggleValueChanged(bool value)
        {
            Debug.Log($"Voice Toggle Value :{value}");
            if (value)
            {
                //NetworkManager.Singleton.GetComponent<VivoxPlayer>().Join3DChannelAsync();
            }
            else
            {
                NetworkManager.Singleton.GetComponent<VivoxPlayer>().LeaveChannelAsync();
            }
        }

        #endregion

        #region Public Methods


        #endregion
    }
}
