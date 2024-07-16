using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VeganVR.Player.Local;
using QFSW.QC;
using VeganVR.UI;

public class SideSelectionUI : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private Button attackingSelectionBtn;
    [SerializeField] private Button defendingSelectionBtn;

    [SerializeField] private TextMeshProUGUI attackSideStateLabel;
    [SerializeField] private TextMeshProUGUI defendSideStateLabel;

    [SerializeField] private CannonRotator cannonRotator;


    // Network Vars
    private NetworkVariable<int> selectedCount = new NetworkVariable<int>(0);

    private NetworkVariable<ulong> attackingPlayerId = new NetworkVariable<ulong>(10,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [HideInInspector]
    public bool IsFirstSideSelectionUISpawnCompleted = false;
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
        //if (!IsServer) return;
        selectedCount.Value = 10;
        Debug.Log("Count Changed to Invalid");
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
        if (GameflowManager.Instance.gameState.Value == GameState.ChoosingSides)
        {
            GameflowManager.Instance.ChangeGameState(GameState.PlayingFirstSet);
        }
        Invoke(nameof(ChangePlayerPos), 1);
    }

    private void OnGameStateNetworkVarChanged(GameState previous, GameState currentState)
    {
        switch(currentState)
        {
            case GameState.ChoosingSides:

                if (!IsFirstSideSelectionUISpawnCompleted) return;
                if (!IsServer) return;

                PlayerPosToSideSelectionClientRpc();
                attackingPlayerId.Value = 10;
                selectedCount.Value = 0;
                ClearSelectSideUiClientRpc();
            break;

            case GameState.PlayingFirstSet:

                if (!IsServer) return;

                EnableAndDisableSideSelectionCanvasClientRpc(false); // Need to Disable the Side Selection Canvas
                ChangeSelectedPlayerCountToInvalid();

                break;

            case GameState.PlayingSecondSet:
              
              if(IsHost)
                SwapPlayersByChangingNetVarAfterFirstRoundServerRpc();
            break;
        }
    }

    #endregion

    #region RPC Methods

    [ClientRpc]
    private void ChangePlayersPosClientRpc()
    {
        Vector3 targetPos = NetworkManager.Singleton.LocalClientId == attackingPlayerId.Value ? NetworkHelper.Instance.AttackingPoint.position : NetworkHelper.Instance.DefendingPoint.position;
        XR_RigRef.instance.ChangePlayerPos(targetPos);
        cannonRotator.TransferCannonOwnershipToClient(attackingPlayerId.Value);
        if (attackingPlayerId.Value == 0) // Blue is Attacking
        {
            Debug.Log("Blue Player Attacking");
        }
        else
        {
            Debug.Log("Red Player Attacking");
        }
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
        Debug.Log("Swap Player Called");
    }

    [ClientRpc]
    private void PlayerPosToSideSelectionClientRpc()
    {
        Vector3 targetPos = NetworkHelper.Instance.SpawnPointList[(int)NetworkManager.Singleton.LocalClientId].position;
        XR_RigRef.instance.ChangePlayerPos(targetPos);
        cannonRotator.TransferCannonOwnershipToClient(0); // To The Host
    }

    [ClientRpc]
    private void ClearSelectSideUiClientRpc()
    {
        attackSideStateLabel.text = "Available";
        defendSideStateLabel.text = "Available";
        attackingSelectionBtn.interactable = true;
        defendingSelectionBtn.interactable = true;
    }

    [ServerRpc,Command]
    private void ChangeGameStateToSecondSetServerRpc()
    {
        if (!IsServer) return;
       GameflowManager.Instance.ChangeGameState(GameState.PlayingSecondSet);
    }

    [ServerRpc,Command]
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

    #endregion
}