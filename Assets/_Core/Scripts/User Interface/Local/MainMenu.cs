using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public static MainMenu instance { get; private set; }

	#region Private Variables

	[Header("Button Refs")]
	[SerializeField] private Button startButton;
	[SerializeField] private Button controlsButton;
	[SerializeField] private Button quitButton;
    [SerializeField] private Button toMainMenu;
    [SerializeField] private InputAction cancelStart;

	[Space(10)]
	[Header("Canvas Refs")]
    [SerializeField] private Transform mainMenuCanvas; // Reference to the Main Menu Canvas
    [SerializeField] private Transform controlsMenuCanvas; // Reference to the Controls Menu Canvas
    [SerializeField] private float transitionDuration = 0.5f; // Duration of the transition

    private Vector3 shownScale = Vector3.one; // Full scale for shown state
    private Vector3 hiddenRotation = new Vector3(0, 0, 10); // Rotation angle for hidden state
    private Vector3 shownRotation = Vector3.zero; // Neutral rotation angle for shown state
    private Vector3 hiddenOffset = new Vector3(0, -100, 0); // Offset position for hidden state
    private Vector3 hiddenScale = new Vector3(0.5f, 0.5f, 0.5f); // Small scale for hidden state

    private Vector3 mainMenuOriginalPosition;
    private Vector3 controlsMenuOriginalPosition;

    [Space(10)]
	[Header("Scene Changing Refs")]
	[SerializeField] private GameObject rightHandGameObj;
	[SerializeField] private GameObject rightHandRay;
	[SerializeField] private MainMenuKatana mainMenuKatana;
	[SerializeField] private GameObject tomato;
	[SerializeField] private CameraFade cameraFade;
	[SerializeField] private RectTransform sliceToStartText;

	private bool isVegeShowing = false;
    private Sequence animationSequence; 

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    private void Awake()
	{
		instance = this;
		startButton.onClick.AddListener(()=>StartButton());
        controlsButton.onClick.AddListener(()=>ToggleControlsMenu());
        toMainMenu.onClick.AddListener(()=>ToggleMainMenu());
		quitButton.onClick.AddListener(() => QuitButton());
	}
	private void Start()
	{
        mainMenuOriginalPosition = mainMenuCanvas.position;
        controlsMenuOriginalPosition = controlsMenuCanvas.position;
        cancelStart.Enable();
        cancelStart.started += CancelStart_started;

        // Optionally, hide the canvases initially
        HideControlsMenuInstant();
    }

    private void CancelStart_started(InputAction.CallbackContext obj)
    {
        if (isVegeShowing)
        {
            StartButton();
        }
    }

    private void Update()
	{

	}

    #endregion

    #region Private Methods
    void StartTextAnimation()
    {
        // Create a new sequence for the shrink and expand animation
        animationSequence = DOTween.Sequence();

        // Add shrink animation
        animationSequence.Append(sliceToStartText.DOScale(0.91f, 0.8f).SetEase(Ease.InOutQuad));

        // Add expand animation
        animationSequence.Append(sliceToStartText.DOScale(1f, 0.8f).SetEase(Ease.InOutQuad));

        // Set the sequence to loop infinitely
        animationSequence.SetLoops(-1, LoopType.Yoyo);
    }

    public void StopTextAnimation()
    {
        // Kill the animation sequence
        if (animationSequence != null)
        {
            animationSequence.Kill();
        }
    }

    private void StartButton()
	{
		isVegeShowing = !isVegeShowing;
		EnableAndDisableSceneChangingObjs(isVegeShowing);
	}
	private void QuitButton()
	{
		cameraFade.TriggerFullFadeIn();
        StopTextAnimation();
        Invoke(nameof(QuitButton),2f);
	}
	private void QuitGame()
	{
		Application.Quit();	
	}
    private void EnableAndDisableSceneChangingObjs(bool value)
    {
        if (tomato)
        {
            tomato.SetActive(value);
        }
        mainMenuKatana.ChangeCanCut(value);
        rightHandGameObj.SetActive(!value);
        rightHandRay.SetActive(!value);


        if (value)
        {
            StartTextAnimation();
            ButtonsInteractableState(false);
        }
        else
        {
            StopTextAnimation();
            ButtonsInteractableState(true);
        }
    }
	private void ToGameScene()
	{
		SceneManager.LoadScene(1);
	}

    private void ToggleMainMenu()
    {
        if (mainMenuCanvas.localScale == hiddenScale)
        {
            ShowMainMenu();
            HideControlsMenu();
        }
        else
        {
            HideMainMenu();
        }
    }

    private void ToggleControlsMenu()
    {
        if (controlsMenuCanvas.localScale == hiddenScale)
        {
            ShowControlsMenu();
            HideMainMenu();
        }
        else
        {
            HideControlsMenu();
        }
    }

    private void ButtonsInteractableState(bool value)
    {
        startButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
        startButton.interactable = value;
        controlsButton.interactable = value;
        quitButton.interactable = value;
    }

    private void ShowMainMenu()
    {
        mainMenuCanvas.DOScale(shownScale, transitionDuration).SetEase(Ease.OutBack);
        mainMenuCanvas.DOLocalRotate(shownRotation, transitionDuration).SetEase(Ease.OutBack);
        mainMenuCanvas.DOMove(mainMenuOriginalPosition, transitionDuration).SetEase(Ease.OutBack);
    }

    private void HideMainMenu()
    {
        mainMenuCanvas.DOScale(hiddenScale, transitionDuration).SetEase(Ease.InBack);
        mainMenuCanvas.DOLocalRotate(hiddenRotation, transitionDuration).SetEase(Ease.InBack);
        mainMenuCanvas.DOMove(mainMenuOriginalPosition + hiddenOffset, transitionDuration).SetEase(Ease.InBack);
    }

    private void ShowControlsMenu()
    {
        controlsMenuCanvas.DOScale(shownScale, transitionDuration).SetEase(Ease.OutBack);
        controlsMenuCanvas.DOLocalRotate(shownRotation, transitionDuration).SetEase(Ease.OutBack);
        controlsMenuCanvas.DOMove(controlsMenuOriginalPosition, transitionDuration).SetEase(Ease.OutBack);
    }

    private void HideControlsMenu()
    {
        controlsMenuCanvas.DOScale(hiddenScale, transitionDuration).SetEase(Ease.InBack);
        controlsMenuCanvas.DOLocalRotate(hiddenRotation, transitionDuration).SetEase(Ease.InBack);
        controlsMenuCanvas.DOMove(controlsMenuOriginalPosition + hiddenOffset, transitionDuration).SetEase(Ease.InBack);
    }

    private void HideMainMenuInstant()
    {
        mainMenuCanvas.localScale = hiddenScale;
        mainMenuCanvas.localRotation = Quaternion.Euler(hiddenRotation);
        mainMenuCanvas.position = mainMenuOriginalPosition + hiddenOffset;
    }

    private void HideControlsMenuInstant()
    {
        controlsMenuCanvas.localScale = hiddenScale;
        controlsMenuCanvas.localRotation = Quaternion.Euler(hiddenRotation);
        controlsMenuCanvas.position = controlsMenuOriginalPosition + hiddenOffset;
    }

    #endregion

    #region Public Methods

    public void ChangeSceneToGame()
	{
		cameraFade.TriggerFullFadeIn();
        StopTextAnimation();
        Invoke(nameof(ToGameScene),2);
	}

    #endregion
}
