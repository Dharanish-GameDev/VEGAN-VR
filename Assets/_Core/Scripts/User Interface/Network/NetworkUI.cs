using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using TMPro;
using VeganVR.VoiceChat;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;


namespace VeganVR.UI
{
    public class NetworkUI : MonoBehaviour
    {
        public static NetworkUI Instance { get; private set; }

        public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
        public class LobbyEventArgs : EventArgs
        {
            public Lobby lobby;
        }
        #region Private Variables

        [SerializeField] private Button createButton;
        [SerializeField] private Button authButton;
        [SerializeField] private TMP_InputField playerNameTextInput;

        [SerializeField] private Toggle voiceToggle;
        [SerializeField] private UnityTransport transport;

        [Space(10)]
        [Header("Lobby")]
        [SerializeField] private Button searchLobbyButton;
        [SerializeField] private GameObject lobbyPrefab;
        [SerializeField] private Transform lobbyPrefabParent;
        [SerializeField] private TextMeshProUGUI lobbyLable;

        [SerializeField] private List <LobbyPrefab> lobbyPrefabList;

        [Space(10)]
        [Header("Relay")]
        [SerializeField] private Button createRelayButton;
        [SerializeField] private Button joinRelayButton;

        private readonly int maxConnections = 2;
        private string relayJoinCode;
        private string playerName;
        private readonly string lobbyName = "VEGAN_VR_LOBBY";
        private readonly string startGameDataKey = "START_GAME";
        private Lobby hostLobby;
        private Lobby joinedLobby;
        private Lobby pollLobby;
        private float heartBeatTimer;
        private float lobbyUpdateTimer;
        private readonly float heartBeatTimerMax = 15;
        private bool isOwnerOfTheLobby = false;


        // Same PC Test
        private string firstName = "Test";
        private string lastName = "Lol";
        private bool haveFirstName = true;

        #endregion

        #region Properties

        public Toggle VoiceToggle => voiceToggle;

        public int MaxConnections => maxConnections;

        #endregion

        #region LifeCycle Methods

        private void Awake()
        {
            PlayerPrefs.DeleteAll();
            Instance = this;
            createButton.onClick.AddListener(() => CreateLobby());
            searchLobbyButton.onClick.AddListener(() => SearchAvailableLobbies());
            authButton.onClick.AddListener(() =>
            {
                VivoxPlayer.Instance.AuthenticateServies(playerNameTextInput.text);
                authButton.interactable = false;
            });
            //createRelayButton.onClick.AddListener(() =>CreateRelay());
            //joinRelayButton.onClick.AddListener(() => JoinRelay(playerNameTextInput.text));
            ChangeLobbyLableTextByValue(0);
            playerName = "Player_" + UnityEngine.Random.Range(10, 99);
            playerNameTextInput.text = firstName;
        }



        private void Update()
        {
            HandleLobbyHeartBeat();
            LobbyPollForUpdates();
#if UNITY_EDITOR

            if(Input.GetKeyDown(KeyCode.RightShift))
            {
                haveFirstName = !haveFirstName;
                if(haveFirstName)
                {
                    playerNameTextInput.text = firstName;
                }
                else
                {
                    playerNameTextInput.text = lastName;
                }
            }
#endif
        }
        #endregion

        #region Private Methods

        private async void CreateLobby()
        {
            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = false,
                    Data = new Dictionary<string, DataObject>
                    {
                    {startGameDataKey,new DataObject(DataObject.VisibilityOptions.Member,"0") }
                    }
                };

                hostLobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxConnections, lobbyOptions);
                joinedLobby = hostLobby;
                isOwnerOfTheLobby = true;
                ChangeUiAfterCreatingLobby(hostLobby);
                
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
           

            //NetworkManager.Singleton.StartHost();
            //HideUI();
        }
        private async void SearchAvailableLobbies()
        {
            try
            {
                QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
                {
                    Count = 4,
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0",QueryFilter.OpOptions.GT)
                    },
                    Order = new List<QueryOrder> 
                    {
                        new QueryOrder(false,QueryOrder.FieldOptions.Created)
                    }

                };


                QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                if(queryResponse.Results.Count > 0)
                {
                    ClearLobbyPrefabList();
                    ChangeLobbyLableTextByValue(2);
                    foreach (var lobby in queryResponse.Results)
                    {
                        CreateLobbyPrefab(lobby,isOwnerOfTheLobby);
                    }
                }
                else
                {
                    ClearLobbyPrefabList();
                    ChangeLobbyLableTextByValue(0);
                }
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
          
        }

        private async void HandleLobbyHeartBeat()
        {
            if(hostLobby == null) return;
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer < 0)
            {
                heartBeatTimer = heartBeatTimerMax;
                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
        public async void JoinLobby(Lobby lobby)
        {
            try
            {
                if (lobby.AvailableSlots == 0) return;
                //Unity.Services.Lobbies.Models.Player player = GetPlayer();

                joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, new JoinLobbyByIdOptions
                {
                    //Player = player
                });
                ////if (joinedLobby == null) return;
                //relayJoinCode = joinedLobby.Data[startGameDataKey].Value;
                //Debug.Log(relayJoinCode);
                //JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                //transport.SetClientRelayData(allocation.RelayServer.IpV4, 
                //    (ushort)allocation.RelayServer.Port, 
                //    allocation.AllocationIdBytes, 
                //    allocation.Key, 
                //    allocation.ConnectionData, 
                //    allocation.HostConnectionData);
                Debug.Log("JOINED LOBBY : " + joinedLobby.Id);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            

        }
        private void ClearLobbyPrefabList()
        {
            if (lobbyPrefabList.Count == 0) return; 
            foreach (var lobbyPrefab in lobbyPrefabList)
            {
                Destroy(lobbyPrefab.gameObject);
            }
            lobbyPrefabList.Clear();
        }
        private void CreateLobbyPrefab(Lobby lobby,bool isOwnerOfTheLobby)
        {
            LobbyPrefab lobbyObject = Instantiate(lobbyPrefab).GetComponent<LobbyPrefab>();
            lobbyObject.transform.SetParent(lobbyPrefabParent, false);
            lobbyObject.UpdateLobbyPrefabUI(lobby,isOwnerOfTheLobby);
            lobbyPrefabList.Add(lobbyObject);
            if(isOwnerOfTheLobby)
            {
                lobbyObject.JoinButton.interactable = false;
            }
        }

        /// <summary>
        /// 0 - No Lobbies Available,1 - Joined Lobby, 2 - Available Lobbies
        /// </summary>
        /// <param name="value"></param>
        private void ChangeLobbyLableTextByValue(int value)
        {
            if (value > 2 || value < 0) return;
            switch(value)
            {
                case 0:
                    lobbyLable.SetText("No Lobbies Available");
                    break;
                case 1:
                    lobbyLable.SetText("Joined Lobby");
                    break;
                case 2:
                    lobbyLable.SetText("Available Lobbies");
                    break;
            }
        }
        private void ChangeUiAfterCreatingLobby(Lobby lobby)
        {
            ChangeLobbyLableTextByValue(1);
            searchLobbyButton.interactable = false;
            createButton.interactable = false;
            ClearLobbyPrefabList();
            CreateLobbyPrefab(lobby, isOwnerOfTheLobby);
        }
        private Unity.Services.Lobbies.Models.Player GetPlayer()
        {
            return new Unity.Services.Lobbies.Models.Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                   {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,playerName) }
                }
            };
        }
        private async void LobbyPollForUpdates()
        {
            if (joinedLobby == null) return;
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = 1.1f;
                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                foreach (var lobbyPrefab in lobbyPrefabList)
                {
                    lobbyPrefab.UpdateLobbyPrefabUI(joinedLobby, isOwnerOfTheLobby);
                }
                if (joinedLobby.Data[startGameDataKey].Value != "0")
                {
                    if(!isOwnerOfTheLobby)
                    {
                        JoinRelay(joinedLobby.Data[startGameDataKey].Value);
                        joinedLobby = null;
                        Debug.Log("Game Started");
                    }
                }
            }
        }
        private async void UpdateRelayCode(string relayCode)
        {
            try
            {
               hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
               {
                    Data = new Dictionary<string, DataObject>
                    {
                        {startGameDataKey,new DataObject(DataObject.VisibilityOptions.Public,relayCode) }
                    }
                });
            }
            catch (Exception ex) 
            {
                Debug.Log(ex);
            }
        }
        private async Task <string> CreateRelay()
        {
            try
            {
                Allocation allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
                string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
                RelayServerData relayServerData = new RelayServerData(allocation,"dtls");
                transport.SetRelayServerData(relayServerData);
                Debug.Log("Game Started");
                NetworkManager.Singleton.StartHost();
                return joinCode;
            }
            catch (Exception ex)
            { 
                Debug.Log(ex);
                return null;
            }
            
        }
        private async void JoinRelay(string joincode)
        {
            try
            {
                JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(joincode);
                RelayServerData relayServerData = new RelayServerData(joinAllocation,"dtls");
                transport.SetRelayServerData(relayServerData);
                Debug.Log("JOINED RELAY WITH JOINCODE : " + joincode);
                NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            
        }

        public async void StartGame()
        {
            if(isOwnerOfTheLobby)
            {
                try
                {
                    string relayCode = await CreateRelay();

                    Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                    {
                        { startGameDataKey,new DataObject(DataObject.VisibilityOptions.Member,relayCode) }
                    }
                    });

                    joinedLobby = lobby;
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                }
               
            }
        }
        #endregion

        #region Public Methods


        #endregion
    }

}
