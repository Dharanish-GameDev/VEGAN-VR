using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VeganVR.Player.Local;

public class WinnerUI : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private Transform blueWinnerCanvas;
	[SerializeField] private Transform redWinnerCanvas;
	[SerializeField] private Transform matchDrawCanvas;
	[SerializeField] private Transform nonVeganCanvas;

	[SerializeField] private Transform b_Text;
	[SerializeField] private Transform r_Text;
	[SerializeField] private Transform d_Text;
	[SerializeField] private Transform nv_Text;


    private Vector3 defenderSideCountingUIFaceVector = new Vector3(0, 180, 0);

	int winnerIdRef;
    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    private void Awake()
	{

	}
	private void Start()
	{

	}
	private void Update()
	{

	}
	
	#endregion

	#region Private Methods

	private void RotateCanvasTowardsLocalPlayer(Transform canvasTransform,int atckId,int winnerId)
    {
        winnerIdRef = winnerId;

        if ((int)NetworkManager.Singleton.LocalClientId == atckId)
        {
            canvasTransform.eulerAngles = Vector3.zero;
        }
        else
        {
            canvasTransform.eulerAngles = defenderSideCountingUIFaceVector;
        }
        if (winnerId > 2) return;

        Invoke(nameof(PlayWinnerTheme),1);

    }

    private void PlayWinnerTheme()
    {
        if ((int)NetworkManager.Singleton.LocalClientId == winnerIdRef )
        {
            // I Win
            SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.winnerTheme, AudioSourceRef.Instance.MiddleFieldSrc, 0.5f);
        }
        else
        {
            // I Lose
            SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.loserTheme, AudioSourceRef.Instance.MiddleFieldSrc, 0.5f);
        }
    }

    private void HideRedWinnerCanvas()
	{
		redWinnerCanvas.gameObject.SetActive(false);
	}

	private void HideMatchDrawCanvas()
	{
		matchDrawCanvas.gameObject.SetActive(false);
	}

	private void HideBlueWinnerCanvas()
	{
		blueWinnerCanvas.gameObject.SetActive(false);
	}

	private void HideNonVeganCanvas()
	{
		nonVeganCanvas.gameObject.SetActive(false);
	}
	#endregion

	#region Public Methods

	public void ShowBluePlayerWinner(int atckId, int winnerInt)
	{
		RotateCanvasTowardsLocalPlayer(blueWinnerCanvas,atckId,winnerInt);
		blueWinnerCanvas.gameObject.SetActive(true);
		AnimateText(b_Text);
		Invoke(nameof(HideBlueWinnerCanvas),5f);
	}

	public void ShowRedPlayerWinner(int atckId,int winnerInt)
	{
        RotateCanvasTowardsLocalPlayer(redWinnerCanvas, atckId, winnerInt);
        redWinnerCanvas.gameObject.SetActive(true);
		AnimateText(r_Text);
		Invoke(nameof(HideRedWinnerCanvas), 5f);
	}

	public void ShowMatchDraw(int atckId)
	{
		RotateCanvasTowardsLocalPlayer(matchDrawCanvas, atckId,2);
		matchDrawCanvas.gameObject.SetActive(true);
		AnimateText(d_Text);
        Invoke(nameof(HideMatchDrawCanvas),5f);
    }

	public void ShowNonVeganUI(int refId)
	{
        RotateCanvasTowardsLocalPlayer(nonVeganCanvas, refId,3);
		nonVeganCanvas.gameObject.SetActive(true);
		AnimateText(nv_Text);
		Invoke(nameof(HideNonVeganCanvas), 4.9f);
    }

    public void AnimateText(Transform textTransform)
    {
        // Reset any previous animations
        textTransform.localScale = Vector3.zero;

        // Sequence of animations
        Sequence mySequence = DOTween.Sequence();

        // Scale up with a smooth effect (1 second)
        mySequence.Append(textTransform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad));

        // Hold for 3 seconds
        mySequence.AppendInterval(3f);

        // Scale down with a smooth effect (1 second)
        mySequence.Append(textTransform.DOScale(Vector3.zero, 1f).SetEase(Ease.InQuad));

        // Play the sequence
        mySequence.Play();
    }
    #endregion
}
