using System;
using System.IO;
using CosmicScavengers.Networking;
using CosmicScavengers.Networking.Extensions;
using CosmicScavengers.Networking.Requests;
using UnityEngine;

public class PlayerEntitiesRequest : MonoBehaviour, INetworkRequest
{
    public NetworkBinaryCommand CommandCode => NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_C;

    public void Dispatch(ClientConnector clientConnector, params object[] parameters)
    {
        if (parameters != null && parameters.Length != 1)
        {
            Debug.LogError(
                "[PlayerEntitiesRequest] Invalid number of parameters for RequestPlayerEntities."
            );
            return;
        }

        long playerId;
        try
        {
            playerId = (long)parameters[0];
        }
        catch (ArgumentNullException ex)
        {
            Debug.LogError(
                "[PlayerEntitiesRequest] Failed to parse parameters for RequestPlayerEntities: "
                    + ex.Message
            );
            return;
        }

        Debug.Log(
            $"[NetworkRequestManager] Sending player entities request for Player ID: {playerId}"
        );

        using var memoryStream = new MemoryStream();
        using var writer = new BinaryWriter(memoryStream);
        writer.WriteShort((short)CommandCode);
        writer.WriteLong(playerId);

        clientConnector.DispatchBinaryMessage(memoryStream.ToArray());
    }
}
