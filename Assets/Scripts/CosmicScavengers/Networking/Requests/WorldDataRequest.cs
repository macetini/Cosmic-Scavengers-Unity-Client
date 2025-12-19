using System;
using System.IO;
using CosmicScavengers.Networking.Extensions;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests
{
    public class WorldStateRequest : MonoBehaviour, INetworkRequest
    {
        public NetworkCommand CommandCode => NetworkCommand.REQUEST_WORLD_STATE_C;

        public void Dispatch(ClientConnector clientConnector, params object[] parameters)
        {
            if (parameters != null && parameters.Length != 1)
            {
                Debug.LogError(
                    "[WorldStateRequest] Invalid number of parameters for RequestWorldState."
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
                    "[WorldStateRequest] Failed to parse parameters for RequestWorldState: "
                        + ex.Message
                );
                return;
            }

            Debug.Log(
                $"[NetworkRequestManager] Sending world state request for Player ID: {playerId}"
            );

            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteShort((short)CommandCode);
            writer.WriteLong(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }
    }
}
