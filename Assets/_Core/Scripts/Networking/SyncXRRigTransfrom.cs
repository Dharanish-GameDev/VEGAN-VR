using UnityEngine;
using Unity.Netcode;
using VeganVR.Player.Local;

namespace VeganVR.Player.Network
{
    public class SyncXRRigTransfrom : NetworkBehaviour
    {
        #region Private Variables

        [SerializeField] private Transform rootTransform;
        [SerializeField] private Transform headTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private Transform playerUiTransform;

        private Vector3 playerUiOffset = new Vector3(0,0.65f,0);
        private Transform anotherPlayerHeadTransform;
        private XR_RigRef xrRigRef = null;
        #endregion

        #region Properties

        public Transform HeadTransform => headTransform;

        public XR_RigRef RigRef => xrRigRef;
        #endregion

        #region LifeCycle Methods

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
            xrRigRef = XR_RigRef.instance;
        }

        

        private void Update()
        {
            SyncPositionAndRotation();
        }
        #endregion

        #region Private Methods

        private void Singleton_OnClientConnectedCallback(ulong obj)
        {
            foreach (var player in FindObjectsOfType<SyncXRRigTransfrom>())
            {
                if (player != this)
                {
                    anotherPlayerHeadTransform = player.HeadTransform;
                }
            }
        }
        private void SyncPositionAndRotation()
        {
            if (!IsOwner) return;

            rootTransform.position = XR_RigRef.instance.RootTransform.position;
            rootTransform.rotation = XR_RigRef.instance.RootTransform.rotation;

            headTransform.position = XR_RigRef.instance.HeadTransform.position;
            headTransform.rotation = XR_RigRef.instance.HeadTransform.rotation;

            leftHandTransform.position = XR_RigRef.instance.LeftHandTransform.position;
            leftHandTransform.rotation = XR_RigRef.instance.LeftHandTransform.rotation;

            rightHandTransform.position = XR_RigRef.instance.RightHandTransform.position;
            rightHandTransform.rotation = XR_RigRef.instance.RightHandTransform.rotation;

            playerUiTransform.position = XR_RigRef.instance.HeadTransform.position + playerUiOffset;

            if (anotherPlayerHeadTransform!= null)
            {
                Vector3 direction = anotherPlayerHeadTransform.position - playerUiTransform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                playerUiTransform.rotation = rotation;
            }
        }

        #endregion

        #region Public Methods



        #endregion
    }
}

