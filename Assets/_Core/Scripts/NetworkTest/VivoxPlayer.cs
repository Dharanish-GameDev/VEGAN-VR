using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.Android;

public class VivoxPlayer : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private Transform vrHeadTransform;

	private string gameVoiceChannel = "VeganVR_VoiceChannel";

	private Channel3DProperties player3DProperties;

	private float nextPosUpdate;

    private int PermissionAskedCount = 0;

    #endregion

    #region Properties



    #endregion

    #region LifeCycle Methods

    private void Start()
    {
        InitializeAsync();
        VivoxService.Instance.LoggedIn += VivoxLoggedIn;
        VivoxService.Instance.LoggedOut += VivoxLoggedOut;
        VivoxService.Instance.ParticipantAddedToChannel += Instance_ParticipantAddedToChannel;
    }

    private void Instance_ParticipantAddedToChannel(VivoxParticipant obj)
    {
        Debug.Log($"Vivox DisplayName : {obj.DisplayName}");
    }

    private void Update()
	{
		if(VivoxService.Instance.ActiveChannels.Count > 0)
        {
            if (Time.time > nextPosUpdate)
            {
                VivoxService.Instance.Set3DPosition(vrHeadTransform.gameObject, gameVoiceChannel);
                nextPosUpdate += 0.5f;
            }
        }
	}
	
	#endregion

	#region Private Methods

	private async void InitializeAsync()
	{
		await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
		await VivoxService.Instance.InitializeAsync();

		Debug.Log("Vivox Initialization Successfull");
	}
    private void VivoxLoggedIn()
    {
        if(VivoxService.Instance.IsLoggedIn)
		{
            Debug.Log("Successfully connected to Vivox");
            Debug.Log("Joining voice channel: " + gameVoiceChannel);
            Join3DChannelAsync();
        }
		else
		{
			Debug.Log("Cannot SignIn to Vivox, Please Check Credentials!");
		}
    }
    private void VivoxLoggedOut()
    {
        LeaveChannelAsync();
        VivoxService.Instance.LogoutAsync();
		Debug.Log("Vivox : LoggedOut From Vivox");
    }

    public async void LeaveChannelAsync()
    {
        await VivoxService.Instance.LeaveAllChannelsAsync();
        Debug.Log("Vivox: Left All Channels");
    }
	public async void Join3DChannelAsync()
	{
		await VivoxService.Instance.JoinPositionalChannelAsync(gameVoiceChannel,ChatCapability.AudioOnly,player3DProperties);
		Debug.Log("Vivox : Successfully Joined 3D Channel");
	}

    private async void VivoxLoginAsync(string displayName)
    {
        LoginOptions loginOptions = new LoginOptions();
        loginOptions.DisplayName = displayName;
        await VivoxService.Instance.LoginAsync(loginOptions);
    }

    #endregion

    #region Public Methods

    public void LoginToVivoxAsync(string displayName)
    {

#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
    bool IsAndroid12AndUp()
    {
        // android12VersionCode is hardcoded because it might not be available in all versions of Android SDK
        const int android12VersionCode = 31;
        AndroidJavaClass buildVersionClass = new AndroidJavaClass("android.os.Build$VERSION");
        int buildSdkVersion = buildVersionClass.GetStatic<int>("SDK_INT");

        return buildSdkVersion >= android12VersionCode;
    }

    string GetBluetoothConnectPermissionCode()
    {
        if (IsAndroid12AndUp())
        {
            // UnityEngine.Android.Permission does not contain the BLUETOOTH_CONNECT permission, fetch it from Android
            AndroidJavaClass manifestPermissionClass = new AndroidJavaClass("android.Manifest$permission");
            string permissionCode = manifestPermissionClass.GetStatic<string>("BLUETOOTH_CONNECT");

            return permissionCode;
        }
        return "";
    }
#endif

        bool IsMicPermissionGranted()
        {
            bool isGranted = Permission.HasUserAuthorizedPermission(Permission.Microphone);
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (IsAndroid12AndUp())
        {
            // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission for all features to work
            isGranted &= Permission.HasUserAuthorizedPermission(GetBluetoothConnectPermissionCode());
        }
#endif
            return isGranted;
        }

        void AskForPermissions()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        if (PermissionAskedCount == 1 && IsAndroid12AndUp())
        {
            permissionCode = GetBluetoothConnectPermissionCode();
        }
#endif
            PermissionAskedCount++;
            Permission.RequestUserPermission(Permission.Microphone);
        }

        bool IsPermissionsDenied()
        {
#if (UNITY_ANDROID && !UNITY_EDITOR) || __ANDROID__
        // On Android 12 and up, we also need to ask for the BLUETOOTH_CONNECT permission
        if (IsAndroid12AndUp())
        {
            return PermissionAskedCount == 2;
        }
#endif
            return PermissionAskedCount == 1;
        }


        if (IsMicPermissionGranted())
        {
            VivoxLoginAsync(displayName);
        }
        else
        {
            if (IsPermissionsDenied())
            {
                PermissionAskedCount = 0;
                VivoxLoginAsync(displayName);
            }
            else
            {
                AskForPermissions();
                VivoxLoginAsync(displayName);
            }
        }

    }
    public void MuteUserMic()
    {
        VivoxService.Instance.MuteInputDevice();
    }
    public void UnMuteUserMic()
    {
        VivoxService.Instance.UnmuteInputDevice();
    }

    #endregion
}
