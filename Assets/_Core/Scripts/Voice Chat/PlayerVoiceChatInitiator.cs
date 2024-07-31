using Unity.Netcode;
using Unity.Services.Vivox;
using VeganVR.UI;

namespace VeganVR.VoiceChat
{
    public class PlayerVoiceChatInitiator : NetworkBehaviour
    {

        #region Private Variables

        string displayName;
        private VivoxParticipant voiceChatParticipant;

        #endregion

        #region Properties

        public VivoxParticipant VoiceChatParticipant => voiceChatParticipant;
        #endregion

        #region LifeCycle Methods

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsOwner) return;
            if (NetworkUI.Instance.IsVoiceChatEnabled)
            {
                VivoxPlayer.Instance.LoginToVivoxAsync();
            }
            else
            {
                //Debug.Log("Player Decided not to Join in Voice Chat");
            }
        }


        
        #endregion

        #region Private Methods


        #endregion

        #region Public Methods


        #endregion
    }
}
