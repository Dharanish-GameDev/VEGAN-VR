using System.Collections;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


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
        [SerializeField] private GameObject testNetworkUICanvasObject;

        [Header("Canvas Refs")]
        [SerializeField] private Canvas sideSelectionCanvas;
        [SerializeField] private Canvas scoreBoardCanvas;
        [SerializeField] private Transform countingCanvasTransform;
        // Test Enum
        public enum NetworkTestEnum
        {
            SamePC,
            Different_PC
        }
        [Space(10)]


        [Header("NetUI Gameobject Ref")]
        [SerializeField] private GameObject netUICanvas;

        [SerializeField] private ScoreCounter scoreCounter;
        [SerializeField] private WinnerUI winnerUI;


        private bool isVoiceChatEnabled = true;
        private Vector3 defenderSideCountingUIFaceVector = new Vector3(0, 180, 0);

        public TextMeshProUGUI countdownText; // Assign the UI Text element in the Inspector
        private int countdownTime; // Countdown start value
        private readonly int countDownTimeMax = 10;

        #endregion

        #region Properties
        public bool IsVoiceChatEnabled => isVoiceChatEnabled;
        public GameObject NetUiCanvas => netUICanvas;

        public ScoreCounter ScoreCounter => scoreCounter;

        public WinnerUI WinnerUI => winnerUI;
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
            EnableAndDisableScoreBoardCanvas(false);
            DisableCountingText();
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public void EnableAndDisableVoiceChat(bool value)
        {
            isVoiceChatEnabled = value;
        }
        public void EnableAndDisableSideSelectionCanvas(bool value) // Turns on and off the Side Selection
        {
            sideSelectionCanvas.enabled = value;
        } 
        public void EnableAndDisableScoreBoardCanvas(bool value) // Turns on and off the Scoreboard
        {
            scoreBoardCanvas.enabled = value;
        }

        public void EnableAndDisableCountingText(bool value)
        {
            countingCanvasTransform.gameObject.SetActive(value);
            if(value)
            {
                countdownTime = countDownTimeMax;
                StartCoroutine(CountdownRoutine());
            }
            Invoke(nameof(DisableCountingText), 11f);
        }

        private void DisableCountingText()
        {
            countingCanvasTransform.gameObject.SetActive(false);
        }

        public void RotateCountingTextTowardsPlayer(int attackingPlayerIdNetVar)
        {
            if ((int)NetworkManager.Singleton.LocalClientId == attackingPlayerIdNetVar)
            {
                countingCanvasTransform.eulerAngles = Vector3.zero;
            }
            else
            {
                countingCanvasTransform.eulerAngles = defenderSideCountingUIFaceVector;
            }
        }

        IEnumerator CountdownRoutine()
        {
            while (countdownTime > 0)
            {
                countdownText.text = countdownTime.ToString();
                AnimateText();
                yield return new WaitForSeconds(1f); // Wait for 1 second
                countdownTime--;
                if(countdownTime == 3)
                {
                    SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.countDownSFX, AudioSourceRef.Instance.MiddleFieldSrc, 0.2f);
                }

            }

            countdownText.text = "GO!";
            AnimateText();
            yield return new WaitForSeconds(1f); // Wait for 1 second before clearing the text
            GameflowManager.Instance.TimerManager.StartTimerServerRpc();
            GameflowManager.Instance.ChangeCanPlayBooleanClientRpc(true);
        }

        void AnimateText()
        {
            // Initial scale to small size to start the animation from there
            countdownText.rectTransform.localScale = Vector3.zero;

            // Scale up with overshoot effect and then scale down smoothly
            countdownText.rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                countdownText.rectTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuad);
            });

            // Fade in the text
            countdownText.DOFade(1, 0.3f);
        }


        #endregion
    }

}
