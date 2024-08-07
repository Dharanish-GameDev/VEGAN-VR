using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class NetAnimateHands : NetworkBehaviour
{
    #region Private Variables

    [SerializeField] private InputActionReference gripInputActionRef;
    [SerializeField] private InputActionReference triggerInputActionRef;

    private float gripStrength;
    private float triggerStrength;

    private Animator handAnimator;

    private readonly int Grip = Animator.StringToHash("Grip");
    private readonly int Trigger = Animator.StringToHash("Trigger");

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    private void Awake()
    {
        handAnimator = GetComponent<Animator>();
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (!IsOwner) return;
        CalculateStrength();
        SetAnimationParameters();
    }

    #endregion

    #region Private Methods

    private void CalculateStrength()
    {
        gripStrength = gripInputActionRef.action.ReadValue<float>();
        triggerStrength = triggerInputActionRef.action.ReadValue<float>();
    }
    private void SetAnimationParameters()
    {
        handAnimator.SetFloat(Grip, gripStrength);
        handAnimator.SetFloat(Trigger, triggerStrength);
    }
    #endregion

    #region Public Methods


    #endregion
}
