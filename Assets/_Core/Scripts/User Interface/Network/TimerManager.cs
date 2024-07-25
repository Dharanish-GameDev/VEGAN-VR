using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;
using QFSW.QC;

public class TimerManager : NetworkBehaviour
{
    // Duration of the timer
    public float timerDuration = 10f; // 10 seconds initially
    private float timer;

    // Reference to the UI Text component
    public TextMeshProUGUI timerText;

    // Event to notify when the timer ends
    public event Action OnTimerEnd;

    private bool isTimerRunning = false;
    private bool isTenSecondsCalled = false;

    private void Start()
    {
        // Initialize the timer at the start
        ResetTimer();
    }

    private void Update()
    {
        if (IsServer && isTimerRunning && timer > 0)
        {
            // Update the timer on the server
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 0;
                isTimerRunning = false;
                TimerEndedClientRpc();
            }
            if(timer <= 10 && !isTenSecondsCalled)
            {
                isTenSecondsCalled = true;
                PlayTenSecondsRemainingClientRpc();
            }
            
            // Update timer on clients every frame
            UpdateTimerClientRpc(timer);
        }
    }

    [ClientRpc]
    private void UpdateTimerClientRpc(float time)
    {
        DisplayTimer(time);
    }

    [ClientRpc]
    private void TimerEndedClientRpc()
    {
        // Check if the timer has ended on the client
        if (OnTimerEnd != null)
        {
            OnTimerEnd.Invoke();
        }

        // Time Expired SFX
        SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.timeExpired, AudioSourceRef.Instance.MiddleFieldSrc, 0.3f);
    }

    [ClientRpc]
    private void PlayTenSecondsRemainingClientRpc()
    {
        SFX_Manager.instance.PlayOneShot(SFX_Manager.instance.GamePlayAudioClips.tenSecondsRemaining, AudioSourceRef.Instance.MiddleFieldSrc, 0.3f);
    }

    private void DisplayTimer(float time)
    {
        // Update the UI Text component with the current timer value
        if (timerText != null)
        {
            timerText.text = FormatTimer(time);
        }
    }

    private string FormatTimer(float time)
    {
        // Format timer display as "0:00"
        return string.Format("0:{0:00}", Mathf.CeilToInt(time));
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTimerServerRpc()
    {
        if (IsServer)
        {
            ResetTimer();
            isTimerRunning = true;
            isTenSecondsCalled = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopTimerServerRpc()
    {
        if (IsServer)
        {
            isTimerRunning = false;
            UpdateTimerClientRpc(timer); // Update clients with the current timer value
        }
    }

    private void ResetTimer()
    {
        timer = timerDuration;
    }
}
