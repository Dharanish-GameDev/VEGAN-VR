using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using QFSW.QC;

public class CannonRotator : NetworkBehaviour
{
    public enum OperatingPlatform
    {
        Oculus,
        PC
    }

    public XRSimpleInteractable leftHandleInteractable;
    public XRSimpleInteractable rightHandleInteractable;
    public float minYaw = -45f; // Minimum yaw rotation angle for the cannon
    public float maxYaw = 45f;  // Maximum yaw rotation angle for the cannon
    public float minPitch = -10f; // Minimum pitch rotation angle for the cannon
    public float maxPitch = 30f;  // Maximum pitch rotation angle for the cannon
    public OperatingPlatform operatingPlatform;

    private bool leftGrabbed = false;
    private bool rightGrabbed = false;
    private Transform leftHand;
    private Transform rightHand;
    private Vector2 rotationVectorPC = new();
    private bool hasOwnership = false;

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

    private void Start()
    {
        if (IsOwner)
        {
            // Initial ownership setup
            hasOwnership = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (hasOwnership)
            {
                ReleaseOwnershipServerRpc();
            }
            else
            {
                RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
            }
        }

        if (!IsOwner) return;

        switch (operatingPlatform)
        {
            case OperatingPlatform.Oculus:
                if (leftGrabbed && rightGrabbed)
                {
                    UpdateOculusRotation(); // Update rotation for Oculus platform
                }
                break;

            case OperatingPlatform.PC:
                HandlePCRotation(); // Handle rotation for PC platform
                break;
        }
    }

    // Oculus platform rotation update
    private void UpdateOculusRotation()
    {
        Vector3 direction = CalculateDirection();
        float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float pitch = Mathf.Asin(direction.y / direction.magnitude) * Mathf.Rad2Deg;

        // Clamp yaw and pitch within specified limits
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Apply rotation to cannon
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        UpdateCannonRotationServerRpc(transform.rotation);
    }

    private Vector3 CalculateDirection()
    {
        Vector3 dir = rightHand.position - leftHand.position;
        dir = transform.InverseTransformDirection(dir);
        return dir;
    }

    // PC platform rotation handling
    private void HandlePCRotation()
    {
        rotationVectorPC.x = Input.GetAxis("Horizontal");
        rotationVectorPC.y = Input.GetAxis("Vertical");

        Vector3 newRotation = transform.rotation.eulerAngles + new Vector3(-rotationVectorPC.y, rotationVectorPC.x, 0f);

        float clampedYaw = ClampAngle(newRotation.y, minYaw, maxYaw);
        float clampedPitch = ClampAngle(newRotation.x, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(clampedPitch, clampedYaw, 0f);
        UpdateCannonRotationServerRpc(transform.rotation);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        if (min <= max)
        {
            return Mathf.Clamp(angle, min, max);
        }
        else
        {
            if (angle > max && angle < min)
            {
                if (angle - max < min - angle)
                    return max;
                else
                    return min;
            }
            return angle;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < -180f)
            angle += 360f;
        else if (angle > 180f)
            angle -= 360f;
        return angle;
    }

    // Server RPC to update cannon rotation
    [ServerRpc(RequireOwnership = false)]
    private void UpdateCannonRotationServerRpc(Quaternion newRotation)
    {
        transform.rotation = newRotation;
        UpdateCannonRotationClientRpc(newRotation);
    }

    // Client RPC to synchronize cannon rotation
    [ClientRpc]
    private void UpdateCannonRotationClientRpc(Quaternion newRotation)
    {
        if (!IsOwner)
        {
            transform.rotation = newRotation;
        }
    }

    // Server RPC to request ownership change
    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        if (IsServer)
        {
            NetworkObject.ChangeOwnership(clientId);
            hasOwnership = true;
            UpdateOwnershipClientRpc(true, clientId); // Pass clientId to update ownership
        }
    }

    // Server RPC to release ownership
    [ServerRpc(RequireOwnership = false)]
    private void ReleaseOwnershipServerRpc()
    {
        if (IsServer && hasOwnership)
        {
            NetworkObject.RemoveOwnership();
            hasOwnership = false;
            UpdateOwnershipClientRpc(false, 0); // Pass 0 to indicate no owner
        }
    }

    // Client RPC to update ownership status
    [ClientRpc]
    private void UpdateOwnershipClientRpc(bool owned, ulong ownerId)
    {
        hasOwnership = owned;

        // Reset grabbed states if ownership is lost
        if (!hasOwnership)
        {
            leftGrabbed = false;
            rightGrabbed = false;
            leftHand = null;
            rightHand = null;
        }

        //Debug.Log($"Cannon Ownership changed to, Owner Client Id: {ownerId}");
    }
    [Command]
    // Method to transfer ownership to a specific client
    public void TransferCannonOwnershipToClient(ulong clientId)
    {
        if (IsServer)
        {
            NetworkObject.ChangeOwnership(clientId);
            hasOwnership = true;
            UpdateOwnershipClientRpc(true, clientId); // Pass clientId to update ownership
        }
    }
}
