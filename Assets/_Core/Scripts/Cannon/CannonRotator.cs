using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CannonRotator : MonoBehaviour
{
    public XRSimpleInteractable leftHandleInteractable;
    public XRSimpleInteractable rightHandleInteractable;
    public float minYaw = -45f; // Minimum yaw rotation angle for the cannon
    public float maxYaw = 45f;  // Maximum yaw rotation angle for the cannon
    public float minPitch = -10f; // Minimum pitch rotation angle for the cannon
    public float maxPitch = 30f;  // Maximum pitch rotation angle for the cannon

    private bool leftGrabbed = false;
    private bool rightGrabbed = false;
    private Transform leftHand;
    private Transform rightHand;

    private void OnEnable()
    {
        leftHandleInteractable.selectEntered.AddListener(OnLeftHandleGrabbed);
        leftHandleInteractable.selectExited.AddListener(OnLeftHandleReleased);
        rightHandleInteractable.selectEntered.AddListener(OnRightHandleGrabbed);
        rightHandleInteractable.selectExited.AddListener(OnRightHandleReleased);
    }

    private void OnDisable()
    {
        leftHandleInteractable.selectEntered.RemoveListener(OnLeftHandleGrabbed);
        leftHandleInteractable.selectExited.RemoveListener(OnLeftHandleReleased);
        rightHandleInteractable.selectEntered.RemoveListener(OnRightHandleGrabbed);
        rightHandleInteractable.selectExited.RemoveListener(OnRightHandleReleased);
    }

    private void OnLeftHandleGrabbed(SelectEnterEventArgs args)
    {
        leftGrabbed = true;
        leftHand = args.interactorObject.transform;
    }

    private void OnLeftHandleReleased(SelectExitEventArgs args)
    {
        leftGrabbed = false;
        leftHand = null;
    }

    private void OnRightHandleGrabbed(SelectEnterEventArgs args)
    {
        rightGrabbed = true;
        rightHand = args.interactorObject.transform;
    }

    private void OnRightHandleReleased(SelectExitEventArgs args)
    {
        rightGrabbed = false;
        rightHand = null;
    }

    private void Update()
    {
        if (leftGrabbed && rightGrabbed)
        {
            UpdateAngles(); // Update the angles based on handle positions
        }
    }

    private void UpdateAngles()
    {
        Vector3 direction = CalculateDirection();
        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg;

        // Clamp the yaw and pitch angles
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Apply the clamped angles to the CannonPivot
        transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }

    private Vector3 CalculateDirection()
    {
        Vector3 dir = rightHand.position - leftHand.position; // Direction between left and right handles
        dir = transform.InverseTransformDirection(dir); // Convert to local space direction
        return dir; // Return direction without normalization for pitch calculation
    }
}
