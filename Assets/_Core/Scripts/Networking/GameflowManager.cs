using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameflowManager : NetworkBehaviour
{
	public static GameflowManager Instance { get; private set; }

	public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.ChoosingSides,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    [SerializeField] private SideSelectionUI selectionUI;

    private void Awake()
    {
        Instance = this;
    }

    #region Public Methods

    public void ChangeGameState(GameState state)
    {
        gameState.Value = state;
    }
    public void ChangeIsFirstSpawnNearSideSelectionUiCompletedToTrue()
    {
        selectionUI.IsFirstSideSelectionUISpawnCompleted = true;
    }
    #endregion
}
public enum GameState
{
    ChoosingSides,
    PlayingFirstSet,
    PlayingSecondSet,
}