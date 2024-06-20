using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace VeganVR.VoiceChat
{
    public class PlayerVoiceChatInitiator : NetworkBehaviour
    {
        #region Private Variables


        #endregion

        #region Properties



        #endregion

        #region LifeCycle Methods

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (NetTestUi.instance.VoiceToggle.isOn)
            {
                NetworkManager.Singleton.GetComponent<VivoxPlayer>().LoginToVivoxAsync("Player : " + OwnerClientId);
            }
            else
            {
                Debug.Log("Player Decided not to Join in Voice Chat");
            }
            NetTestUi.instance.VoiceToggle.onValueChanged.AddListener(VoiceToggleValueChanged);
        }
        #endregion

        #region Private Methods

        private void VoiceToggleValueChanged(bool value)
        {
            Debug.Log($"Voice Toggle Value :{value}");
            if (value)
            {
                //NetworkManager.Singleton.GetComponent<VivoxPlayer>().LoginToVivoxAsync("Player : " + OwnerClientId);
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
