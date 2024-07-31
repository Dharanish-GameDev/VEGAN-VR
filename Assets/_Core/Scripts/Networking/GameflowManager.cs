using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameflowManager : NetworkBehaviour
{
	public static GameflowManager Instance { get; private set; }


    [HideInInspector]
	public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.ChoosingSides,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    [SerializeField] private PlayerSideManagerNet playerSideManager;
    [SerializeField] private CameraFade cameraFade;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private List<UnityEngine.GameObject> slicabbleBlockersList = new List<UnityEngine.GameObject>();
    [SerializeField] private List<Net_Sword> katanasList = new List<Net_Sword>();

    private bool canPlayGame = false;

    public TimerManager TimerManager => timerManager;
    public List<UnityEngine.GameObject> SlicabbleBlockerList => slicabbleBlockersList;
    public bool CanPlayGame => canPlayGame;

    public PlayerSideManagerNet PlayerSideManager 
    {
        get 
        {
            return playerSideManager;
        }
    }
    

    public CameraFade CameraFade => cameraFade;
    private void Awake()
    {
        Instance = this;
        timerManager.OnTimerEnd += TimerManager_OnTimerEnd;
    }

    private void TimerManager_OnTimerEnd()
    {
        if (!IsServer) return;
        ChangeCanPlayBooleanClientRpc(false);
        if(gameState.Value == GameState.PlayingFirstSet)
        {
            Invoke(nameof(ChangeGameStateToSecondSetServerRpc),2);
        }
        else if(gameState.Value == GameState.PlayingSecondSet)
        {
            Invoke(nameof(ChangeGameStateChoosingSideServerRpc), 2);
        }
    }

    #region Public Methods


    [ServerRpc]
    private void ChangeGameStateToSecondSetServerRpc()
    {
        ChangeGameState(GameState.PlayingSecondSet);
        PlaySwitcingSidesSFXClientRpc();
    }

    [ServerRpc]
    private void ChangeGameStateChoosingSideServerRpc()
    {
        ChangeGameState(GameState.ChoosingSides);
    }

    public void ChangeGameState(GameState state)
    {
        gameState.Value = state;
    }
    public void ChangeIsFirstSpawnNearSideSelectionUiCompletedToTrue()
    {
        playerSideManager.IsFirstSideSelectionUISpawnCompleted = true; 
    }

    [ClientRpc]
    public void ChangeCanPlayBooleanClientRpc(bool value)
    {
        canPlayGame = value;
    }

    public void ChangeGameState()
    {
        if (!IsServer) return;
        if (gameState.Value == GameState.PlayingFirstSet)
        {
            ChangeGameStateToSecondSetServerRpc();
        }
        else if (gameState.Value == GameState.PlayingSecondSet)
        {
            ChangeGameStateChoosingSideServerRpc();
        }
    }

    public void ChangeKatanasPosToInitial()
    {
        foreach(var katana in katanasList)
        {
            katana.KatanaTranformToInitial();
        }
    }

    [ClientRpc]
    public void PlaySwitcingSidesSFXClientRpc()
    {
        SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.switchingSides, AudioSourceRef.Instance.MiddleFieldSrc, 0.25f);
    }
    #endregion
}
public enum GameState
{
    ChoosingSides,
    PlayingFirstSet,
    PlayingSecondSet,
}