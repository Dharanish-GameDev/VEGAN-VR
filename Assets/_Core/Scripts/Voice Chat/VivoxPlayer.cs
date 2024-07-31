using UnityEngine;
using Unity.Services.Vivox;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine.Android;
using System;

namespace VeganVR.VoiceChat
{
    public class VivoxPlayer : MonoBehaviour
    {
        public static VivoxPlayer Instance { get; private set; }

        public event Action OnVivoxInitiated;

        #region Private Variables

        [SerializeField] private Transform vrHeadTransform;

        private readonly string gameVoiceChannel = "VeganVR_VoiceChannel";

        private Channel3DProperties player3DProperties;

        private float nextPosUpdate;

        private int PermissionAskedCount = 0;

        private bool isJoinedChannel = false;

        #endregion

        #region Properties



        #endregion

        #region LifeCycle Methods

        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            //AuthenticateServies();

            VivoxService.Instance.LoggedIn += VivoxLoggedIn;
            VivoxService.Instance.LoggedOut += VivoxLoggedOut;
            VivoxService.Instance.ChannelLeft += VivoxChannelLeft;
            VivoxService.Instance.ConnectionFailedToRecover += Vivox_ConnectionFailedToRecover;

            VivoxService.Instance.ChannelJoined += VivoxChannelJoined;
        }

        

        private void OnDisable()
        {
            VivoxService.Instance.ChannelJoined -= VivoxChannelJoined;
            VivoxService.Instance.LoggedIn -= VivoxLoggedIn;
            VivoxService.Instance.LoggedOut -= VivoxLoggedOut;
        }
        private void Update()
        {
            if (!isJoinedChannel) return;

            if (Time.time > nextPosUpdate)
            {
                try
                {
                    if (VivoxService.Instance.ActiveChannels.Count > 0)
                    {
                        VivoxService.Instance.Set3DPosition(vrHeadTransform.gameObject, gameVoiceChannel);
                    }
                    
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                }
                
                nextPosUpdate += 0.5f;
            }
        }

        #endregion

        #region Private Methods

        public async void AuthenticateServies(string playerName)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () => {
                // do nothing
                Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);

            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            try
            {
                await VivoxService.Instance.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            OnVivoxInitiated?.Invoke();

        }
        private void VivoxLoggedIn()
        {
            if (VivoxService.Instance.IsLoggedIn)
            {
                Debug.Log("<color=red>Successfully connected to Vivox</color>");
                Join3DChannelAsync();
            }
            else
            {
                Debug.Log("Cannot SignIn to Vivox, Please Check Credentials!");
            }
        }

        private async void VivoxLoginAsync()
        {   
            try
            {
                await VivoxService.Instance.LoginAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            
        }

        private async void VivoxLoggedOut()
        {
            LeaveChannelAsync();
            try
            {
                await VivoxService.Instance.LogoutAsync();
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
           
            Debug.Log("Vivox : LoggedOut From Vivox");
        }

        private void VivoxChannelLeft(string obj)
        {
            isJoinedChannel = false;
        }

        private void VivoxChannelJoined(string obj)
        {
            isJoinedChannel = true;
        }
        private void Vivox_ConnectionFailedToRecover()
        {
            isJoinedChannel = false;
        }

        #endregion

        #region Public Methods

        public void LoginToVivoxAsync()
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
                VivoxLoginAsync();
            }
            else
            {
                if (IsPermissionsDenied())
                {
                    PermissionAskedCount = 0;
                    VivoxLoginAsync();
                }
                else
                {
                    AskForPermissions();
                    VivoxLoginAsync();
                }
            }

        }

        public async void Join3DChannelAsync()
        {
            try
            {
                Debug.Log("<color=yellow>Joining Voice Channel!</color>");
                await VivoxService.Instance.JoinPositionalChannelAsync(gameVoiceChannel, ChatCapability.AudioOnly, player3DProperties);
                Debug.Log("<color=green>Joined Voice Channel!</color>");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
           
        }

        public async void LeaveChannelAsync()
        {
            try
            {
                await VivoxService.Instance.LeaveAllChannelsAsync();
                Debug.Log("Vivox: Left All Channels");
                isJoinedChannel = false;
            }
            catch(Exception ex)
            {
                Debug.Log(ex);
            }
            
        }

        public async void SetActiveInputDevice(int deviceIndex)
        {
            if (deviceIndex >= 0 && deviceIndex < VivoxService.Instance.AvailableInputDevices.Count)
            {
                try
                {
                    var device = VivoxService.Instance.AvailableInputDevices[deviceIndex];
                    await VivoxService.Instance.SetActiveInputDeviceAsync(device);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
                
            }
            else
            {
                Debug.LogError("Invalid device index");
            }
        }

        #endregion
    }

}
