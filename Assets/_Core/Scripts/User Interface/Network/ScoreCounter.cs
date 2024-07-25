using TMPro;
using Unity.Netcode;
using UnityEngine;
using VeganVR.UI;

public class ScoreCounter : NetworkBehaviour
{
    [HideInInspector]
    NetworkVariable<int> BluePlayerScore = new NetworkVariable<int>(0);
    [HideInInspector]
    NetworkVariable<int> RedPlayerScore = new NetworkVariable<int>(0);

    [SerializeField] private TextMeshProUGUI bluePlayerScoreText;
    [SerializeField] private TextMeshProUGUI redPlayerScoreText;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        bluePlayerScoreText.text = "00";
        redPlayerScoreText.text = "00";
        BluePlayerScore.OnValueChanged += BluePlayerScoreChanged;
        RedPlayerScore.OnValueChanged += RedPlayerScoreChanged;
    }


    private void BluePlayerScoreChanged(int prev,int newScore)
    {
        bluePlayerScoreText.SetText(newScore.ToString("D2"));
    }

    private void RedPlayerScoreChanged(int prev,int newScore)
    {
        redPlayerScoreText.SetText(newScore.ToString("D2"));
    }



    // Method to add score to BluePlayer (host)
    [ServerRpc(RequireOwnership = false)]
    private void AddScoreToBluePlayerServerRpc()
    {
        BluePlayerScore.Value += 1;
        Debug.Log("Blue Player Score Added");
    }

    // Method to add score to RedPlayer (client)
    [ServerRpc(RequireOwnership = false)]
    private void AddScoreToRedPlayerServerRpc()
    {
        RedPlayerScore.Value += 1;
        Debug.Log("Red Player Score Added");
    }

    // Method to clear both players' scores
    [ServerRpc(RequireOwnership = false)]
    public void ClearScoresServerRpc()
    {
        BluePlayerScore.Value = 0;
        RedPlayerScore.Value = 0;
    }

    // Example method to add score (can be called from any script)
    public void AddScore(ulong playerId)
    {
        if (IsServer)
        {
            if (playerId == 0)
            {
                AddScoreToBluePlayerServerRpc();
            }
            else if (playerId == 1)
            {
                AddScoreToRedPlayerServerRpc();
            }
        }
    }

    // Example method to clear scores (can be called from any script)
    public void ClearScores()
    {
        if (IsServer)
        {
            ClearScoresServerRpc();
        }
    }

    [ClientRpc]
    public void CheckForHighScoresClientRpc(int attackingPlayerId)
    {
        int winnerInt = BluePlayerScore.Value > RedPlayerScore.Value ? 0 : 1;
        if(BluePlayerScore.Value > RedPlayerScore.Value)
        {
            NetworkUI.Instance.WinnerUI.ShowBluePlayerWinner(attackingPlayerId,winnerInt);
        }
        else if(BluePlayerScore.Value == RedPlayerScore.Value)
        {
            NetworkUI.Instance.WinnerUI.ShowMatchDraw(attackingPlayerId);
        }
        else
        {
            NetworkUI.Instance.WinnerUI.ShowRedPlayerWinner(attackingPlayerId, winnerInt);
        }

        ClearScores();
    }
}
