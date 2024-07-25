using Unity.Netcode;
using UnityEngine;
using System.Collections;
using TMPro;

public class PingDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pingText;
    private Coroutine coroutine;
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        coroutine = StartCoroutine(UpdateRttCoroutine());
    }

    private void Singleton_OnServerStarted()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            pingText.text = "Zero ms";
            StopCoroutine(coroutine);
        }
    }

    private IEnumerator UpdateRttCoroutine()
    {
        while (true)
        {
            if (NetworkManager.Singleton.IsClient)
            {
                // Get the current RTT to the server
                float currentRtt = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);
                pingText.text = currentRtt + " ms";
            }

            // Wait for 1 second before updating again
            yield return new WaitForSeconds(2.0f);
        }
    }
}