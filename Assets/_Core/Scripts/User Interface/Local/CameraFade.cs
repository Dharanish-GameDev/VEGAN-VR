using UnityEngine;
using DG.Tweening;

public class CameraFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float duration = 1f;

    void Start()
    {
       // InitialCameraFade();
    }

    private void InitialCameraFade()
    {
        canvasGroup.DOFade(0, 2);
    }

    public void TriggerFade()
    {
        // Animate alpha to 1 (fade in)
        canvasGroup.DOFade(1f, duration).OnComplete(() =>
        {
            // Animate alpha to 0 (fade out) after fade in completes
            canvasGroup.DOFade(0f, duration);
        });
    }
}
