using Unity.Netcode;
using UnityEngine;

public class CustomClientNetworkTransform : NetworkBehaviour
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Update()
    {
        if (IsOwner) 
        {
            SendPositionToServerRpc(transform.position);
            SendRotationToServerRpc(transform.rotation);
        }
        else 
        {
            transform.position = networkPosition;
            transform.rotation = networkRotation;
        }
    }

    [ServerRpc]
    private void SendPositionToServerRpc(Vector3 position)
    {
        SendPositionFromClientRpc(position);
    }

    [ClientRpc]
    private void SendPositionFromClientRpc(Vector3 position)
    {
        if (IsOwner)
            return;

        networkPosition = position;
    }

    [ServerRpc]
    private void SendRotationToServerRpc(Quaternion rotation)
    {
        SendRotationFromClientRpc(rotation);
    }

    [ClientRpc]
    private void SendRotationFromClientRpc(Quaternion rotation)
    {
        if (IsOwner)
            return;

       networkRotation = rotation;
    }
}
