using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using VeganVR.UI;

public class LobbyPrefab : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI availableSlotsText;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button startButton;

    private Lobby lobby;

    #region Properties

    public Button JoinButton => joinButton;
    #endregion

    private void Awake()
    {
        joinButton.onClick.AddListener(() => NetworkUI.Instance.JoinLobby(lobby));
        startButton.onClick.AddListener(() => NetworkUI.Instance.StartGame());
    }

    #region Public Methods

    public void UpdateLobbyPrefabUI(Lobby lobby,bool isOwnerOfTheLobby)
    {
        this.lobby = lobby;
        lobbyNameText.text = lobby.Name;
        availableSlotsText.text = lobby.AvailableSlots.ToString() + " Slots";
        if(isOwnerOfTheLobby)
        {
            joinButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }

    public void SetLobbyNameText(string text)
    {
        lobbyNameText.SetText(text);
    }
    public void SetAvailableSlotsCountText(int count)
    {
        availableSlotsText.SetText(count.ToString()+" Slot");
    }
    #endregion
}
