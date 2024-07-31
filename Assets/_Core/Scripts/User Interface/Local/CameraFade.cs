using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class CameraFade : MonoBehaviour
{
    [Header("Continuos Turn Action")]
    public InputAction turnAction;
    [Space(10)]
    public CanvasGroup canvasGroup;
    public float duration = 1f;
    private bool isTurning = false;

    void OnEnable()
    {
        if (turnAction != null)
        {
            turnAction.performed += OnPrimary2DAxis;
            turnAction.canceled += OnPrimary2DAxis;
            turnAction.Enable();
        }
    }

    void OnDisable()
    {
        if (turnAction != null)
        {
            turnAction.performed -= OnPrimary2DAxis;
            turnAction.canceled -= OnPrimary2DAxis;
            turnAction.Disable();
        }
    }
    private void Start()
    {
        InitialCameraFade();
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

    public void OnPrimary2DAxis(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        // Check only the horizontal axis (x-axis)
        if (Mathf.Abs(input.x) > 0.1f && !isTurning)
        {
            isTurning = true;
            TriggerFadeIn();
        }
        else if (Mathf.Abs(input.x) <= 0.1f && isTurning)
        {
            isTurning = false;
            TriggerFadeOut();
        }
    }

    public void TriggerFadeIn()
    {
        canvasGroup.DOFade(0.7f, 1);
    }
    public void TriggerFullFadeIn()
    {
        canvasGroup.DOFade(1, 1.8f);
    }
    public void TriggerFadeOut()
    {
        canvasGroup.DOFade(0f, 1);
    }
}
