using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VeganVR.Player.Local;
using QFSW.QC;
using VeganVR.UI;

public class PlayerSideManagerNet : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private Button attackingSelectionBtn;
    [SerializeField] private Button defendingSelectionBtn;

    [SerializeField] private TextMeshProUGUI attackSideStateLabel;
    [SerializeField] private TextMeshProUGUI defendSideStateLabel;


    // Network Vars
    private NetworkVariable<int> selectedCount = new NetworkVariable<int>(0);

    private NetworkVariable<ulong> attackingPlayerId = new NetworkVariable<ulong>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [HideInInspector]
    public bool IsFirstSideSelectionUISpawnCompleted = false;

    private int localAttackingPlayerId;
    private SFX_Manager sfx_Manager;
    private AudioSourceRef audioSourceRef;


    public int AttackingPlayerId => (int)attackingPlayerId.Value;
    private void Awake()
    {
        // Add listeners to buttons
        attackingSelectionBtn.onClick.AddListener(() =>
        {
            UpdateAttackingSelectedServerRpc(NetworkManager.Singleton.LocalClientId);
            defendingSelectionBtn.interactable = false;
            attackSideStateLabel.text = "Selected";
        });

        defendingSelectionBtn.onClick.AddListener(() =>
        {
            UpdateDefendingSelectedServerRpc(NetworkManager.Singleton.LocalClientId);
            attackingSelectionBtn.interactable = false;
            defendSideStateLabel.text = "Selected";
        });
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        GameflowManager.Instance.gameState.OnValueChanged += OnGameStateNetworkVarChanged;
        selectedCount.OnValueChanged += OnSelectedPlayerCountValueChanged;
        sfx_Manager = SFX_Manager.instance;
        audioSourceRef = AudioSourceRef.Instance;
    }

    public override void OnDestroy()
    {
        GameflowManager.Instance.gameState.OnValueChanged -= OnGameStateNetworkVarChanged;
        selectedCount.OnValueChanged -= OnSelectedPlayerCountValueChanged;
    }


    #region Private Methods

    // Changes the Players Pos Using RPC by Checking that Net Var
    private void ChangePlayerPos()
    {
        ChangePlayersPosClientRpc();
    }

    //To Change to Selected count to Invalid for Second Set Spawning
    private void ChangeSelectedPlayerCountToInvalid()
    {
        selectedCount.Value = 10;
    }

    //To Change to Selected count to valid for swapping pos
    private void ChangeSelectedPlayerCountToValid()
    {
        if (!IsServer) return;
        selectedCount.Value = 2;
    }

    // Checks the Selectd Count value is Zero and then Changes the Player Pos
    private void OnSelectedPlayerCountValueChanged(int last, int newValue)
    {
        if (newValue != 2) return;
        if (!IsServer) return;
        DoCameraFadeClientRpc();
        if (GameflowManager.Instance.gameState.Value == GameState.ChoosingSides)
        {
            GameflowManager.Instance.ChangeGameState(GameState.PlayingFirstSet);
        }
        Invoke(nameof(ChangePlayerPos), 1);
    }

    private void OnGameStateNetworkVarChanged(GameState previous, GameState currentState)
    {
        switch (currentState)
        {
            case GameState.ChoosingSides:

                if (!IsFirstSideSelectionUISpawnCompleted) return;

                //Local
                Invoke(nameof(PlayLobbySFX),7);

                if (!IsServer) return;
                ChangeKatanasPosToInitialServerRpc();
                localAttackingPlayerId = (int)attackingPlayerId.Value;
                NetworkUI.Instance.ScoreCounter.CheckForHighScoresClientRpc(localAttackingPlayerId);
                Invoke(nameof(DoCameraFadeClientRpc),6);
                Invoke(nameof(ChangePlayerPosToSelectionSide),7);
                attackingPlayerId.Value = 10;
                selectedCount.Value = 0;
                ClearSelectSideUiClientRpc();
                EnableAndDisableScoreBoardCanvasClientRpc(false);
                EnableAndDisableSideSelectionCanvasClientRpc(true);
                break;

            case GameState.PlayingFirstSet:

                //Local
                SFX_Manager.instance.ChangeClipsOnLoopingAudioSrc(sfx_Manager.GamePlayAudioClips.gamePlayMusic,audioSourceRef.BG_AudioSrc,0.05f);

                if (!IsServer) return;
                EnableAndDisableSideSelectionCanvasClientRpc(false); // Need to Disable the Side Selection Canvas
                EnableAndDisableScoreBoardCanvasClientRpc(true);
                ChangeKatanasPosToInitialServerRpc();
                Invoke(nameof(EnableCountingText), 1);
                ChangeSelectedPlayerCountToInvalid();


                break;

            case GameState.PlayingSecondSet:

                if (!IsHost) return;
                ChangeKatanasPosToInitialServerRpc();
                SwapPlayersByChangingNetVarAfterFirstRoundServerRpc();
                Invoke(nameof(EnableCountingText), 1);
                break;
        }
    }


    private void EnableCountingText()
    {
        EnableAndDisableCountingTextClientRpc(true);
        localAttackingPlayerId = (int)attackingPlayerId.Value;
        RotateCountingTextClientRpc(localAttackingPlayerId);
    }

    private void ChangePlayerPosToSelectionSide()
    {
        PlayerPosToSideSelectionClientRpc();
    }

    private void PlayLobbySFX()
    {
        SFX_Manager.instance.ChangeClipsOnLoopingAudioSrc(sfx_Manager.GamePlayAudioClips.lobbyMusic, audioSourceRef.BG_AudioSrc, 0.22f);
    }

    #endregion

    #region RPC Methods 

    [ClientRpc]
    private void ChangePlayersPosClientRpc()
    {
        Vector3 targetPos = NetworkManager.Singleton.LocalClientId == attackingPlayerId.Value ? NetworkHelper.Instance.AttackingPoint.position : NetworkHelper.Instance.DefendingPoint.position;
        XR_RigRef.instance.ChangePlayerPos(targetPos);
        CannonRotator.Instance.TransferCannonOwnershipToClient(attackingPlayerId.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateDefendingSelectedServerRpc(ulong playerId)
    {
        selectedCount.Value = selectedCount.Value + 1;
        UpdateDefendingSelectedClientRpc();
    }

    [ClientRpc]
    private void UpdateDefendingSelectedClientRpc()
    {
        defendSideStateLabel.text = "Occupied";
        defendingSelectionBtn.interactable = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateAttackingSelectedServerRpc(ulong playerId)
    {
        attackingPlayerId.Value = playerId;
        selectedCount.Value = selectedCount.Value + 1;
        UpdateAttackingSelectedClientRpc();
    }

    [ClientRpc]
    private void UpdateAttackingSelectedClientRpc()
    {
        attackSideStateLabel.text = "Occupied";
        attackingSelectionBtn.interactable = false;
    }

    // This Method will swap the Player Pos from attacking to defending and ViveVersa after FirstSet Completed
    [ServerRpc]
    private void SwapPlayersByChangingNetVarAfterFirstRoundServerRpc()
    {
        if (!IsServer) return;

        if (attackingPlayerId.Value == 0)
        {
            attackingPlayerId.Value = 1;
        }
        else if (attackingPlayerId.Value == 1)
        {
            attackingPlayerId.Value = 0;
        }

        ChangeSelectedPlayerCountToValid();
    }

    [ClientRpc]
    private void PlayerPosToSideSelectionClientRpc()
    {
        Vector3 targetPos = NetworkHelper.Instance.SpawnPointList[(int)NetworkManager.Singleton.LocalClientId].position;
        XR_RigRef.instance.ChangePlayerPos(targetPos);
        CannonRotator.Instance.TransferCannonOwnershipToClient(0); // To The Host
        CannonRotator.Instance.EnableAndDisableDrawTrajectory(false);
    }

    [ClientRpc]
    private void ClearSelectSideUiClientRpc()
    {
        attackSideStateLabel.text = "Available";
        defendSideStateLabel.text = "Available";
        attackingSelectionBtn.interactable = true;
        defendingSelectionBtn.interactable = true;
    }

    [ServerRpc(RequireOwnership = false), Command]
    private void ChangeGameStateToSecondSetServerRpc()
    {
        if (!IsServer) return;
        GameflowManager.Instance.ChangeGameState(GameState.PlayingSecondSet);
    }

    [ServerRpc(RequireOwnership = false), Command]
    private void ChangeGameStateChoosingSideServerRpc()
    {
        if (!IsServer) return;
        GameflowManager.Instance.ChangeGameState(GameState.ChoosingSides);
    }

    [ClientRpc]
    private void EnableAndDisableSideSelectionCanvasClientRpc(bool value)
    {
        NetworkUI.Instance.EnableAndDisableSideSelectionCanvas(value);
    }

    [ClientRpc]
    private void EnableAndDisableScoreBoardCanvasClientRpc(bool value)
    {
        NetworkUI.Instance.EnableAndDisableScoreBoardCanvas(value);
    }

    [ClientRpc]
    private void EnableAndDisableCountingTextClientRpc(bool value)
    {
        NetworkUI.Instance.EnableAndDisableCountingText(value);
    }

    [ClientRpc]
    private void RotateCountingTextClientRpc(int value)
    {
        NetworkUI.Instance.RotateCountingTextTowardsPlayer(value);
    }

    [ClientRpc]
    private void DoCameraFadeClientRpc()
    {
        GameflowManager.Instance.CameraFade.TriggerFade();
    }


    [ServerRpc]
    private void ChangeKatanasPosToInitialServerRpc()
    {
        ChangeKatanasPosToInitialClientRpc();
    }
    [ClientRpc]
    private void ChangeKatanasPosToInitialClientRpc()
    {
        XR_RigRef.instance.DetachInteractablesFromDirectInteractors();
        Invoke(nameof(ChangeKatanaInitialPos), 1);
    }

    private void ChangeKatanaInitialPos()
    {
        GameflowManager.Instance.ChangeKatanasPosToInitial();
    }

    #endregion
}