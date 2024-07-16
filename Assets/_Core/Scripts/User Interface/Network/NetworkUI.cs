using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;


namespace VeganVR.UI
{
    public class NetworkUI : MonoBehaviour
    {
        public static NetworkUI Instance { get; private set; }

        #region Private Variables
        [Header("Test To be removed")]
        [SerializeField] private Button localCreateButton;
        [SerializeField] private Button localJoinButton;
        [SerializeField] private NetworkTestEnum networkTestEnum;
        [SerializeField] private Canvas sideSelectionCanvas;
        [SerializeField] private GameObject testNetworkUICanvasObject;
        // Test Enum
        public enum NetworkTestEnum
        {
            SamePC,
            Different_PC
        }
        [Space(10)]


        [Header("NetUI Gameobject Ref")]
        [SerializeField] private GameObject netUICanvas;


        private bool isVoiceChatEnabled = true;

        #endregion

        #region Properties
        public bool IsVoiceChatEnabled => isVoiceChatEnabled;
        public GameObject NetUiCanvas => netUICanvas;
        #endregion

        #region LifeCycle Methods

        private void Awake()
        {
            PlayerPrefs.DeleteAll();
            Instance = this;
        }

        private void Start()
        {
            // Test
            switch (networkTestEnum)
            {
                case NetworkTestEnum.SamePC:
                    localCreateButton.onClick.AddListener(() =>
                    {
                        NetworkManager.Singleton.StartHost();
                        testNetworkUICanvasObject.SetActive(false);
                    });
                    localJoinButton.onClick.AddListener(() =>
                    {
                        NetworkManager.Singleton.StartClient();
                        testNetworkUICanvasObject.SetActive(false);
                    });
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", 7777);
                    isVoiceChatEnabled = false;
                    netUICanvas.SetActive(false);
                break;

                case NetworkTestEnum.Different_PC:
                    localCreateButton.gameObject.SetActive(false);
                    localJoinButton.gameObject.SetActive(false);
                    isVoiceChatEnabled = true;
                break;
            }
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public void EnableAndDisableVoiceChat(bool value)
        {
            isVoiceChatEnabled = value;
        }

        public void EnableAndDisableSideSelectionCanvas(bool value)
        {
            sideSelectionCanvas.enabled = value;
        }

        #endregion
    }

}
