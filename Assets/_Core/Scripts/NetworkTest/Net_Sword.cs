using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class Net_Sword : NetworkBehaviour
{
	[SerializeField] private Katana katanaGrabbable;
}
