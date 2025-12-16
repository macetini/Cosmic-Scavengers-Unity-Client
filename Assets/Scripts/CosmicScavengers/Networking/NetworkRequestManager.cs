using System;
using System.Collections.Generic;
using System.IO;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Extensions;
using CosmicScavengers.Networking.Handlers;
using UnityEngine;

namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Listens for high-level game events and translates them into network requests.
    /// This acts as a bridge between the game logic and the low-level ClientConnector.
    /// </summary>
    public class NetworkRequestManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        [Tooltip("The ClientConnector responsible for low-level network communication.")]
        private ClientConnector clientConnector;

        [SerializeField]
        [Tooltip("Event channel to notify when the player is authenticated.")]
        private GetWorldDataEventChannel getWorldDataEventChannel;

        public EntitiesUpdateEventChannel entitiesUpdateEventChannel;

        void Start()
        {
            if (clientConnector == null)
            {
                throw new Exception(
                    "[NetworkRequestManager] ClientConnector dependency is not assigned."
                );
            }

            clientConnector.OnBinaryMessageReceived += HandleBinaryMessage;
        }

        void OnDestroy()
        {
            if (clientConnector != null)
            {
                clientConnector.OnBinaryMessageReceived -= HandleBinaryMessage;
            }
        }

        private void HandleBinaryMessage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning("[NetworkRequestManager] Received empty binary message.");
                return;
            }

            int payloadLength;

            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);
            short command = reader.ReadShort();
            switch (command)
            {
                case NetworkCommands.REQUEST_WORLD_DATA_S:
                    payloadLength = reader.ReadInt();
                    byte[] worldDataBytes = reader.ReadBytes(payloadLength);
                    WorldClientDataHandler.Handle(worldDataBytes);
                    //getWorldDataEventChannel.Raise(worldData);
                    break;
                case NetworkCommands.REQUEST_PLAYER_ENTITIES_S:
                    payloadLength = reader.ReadInt();
                    int count = reader.ReadInt();
                    byte[] playerEntitiesBytes = reader.ReadBytes(payloadLength - sizeof(int));
                    PlayerEntitiesDataHandler.Handle(playerEntitiesBytes, count);
                    //entitiesUpdateEventChannel.Raise(playerEntities);
                    break;
                default:
                    Debug.LogWarning(
                        "[NetworkRequestManager] Unhandled command received: " + command
                    );
                    break;
            }
        }

        /// <summary>
        /// Sends a request to the server to retrieve the initial world state for the authenticated player.
        ///
        /// <param name="playerId">The ID of the authenticated player.</param>
        ///
        /// </summary>
        public void OnRequestWorldState(long playerId)
        {
            Debug.Log(
                $"[NetworkRequestManager] Sending world state request for Player ID: {playerId}"
            );
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteShort(NetworkCommands.REQUEST_WORLD_DATA_C);
            writer.WriteLong(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }

        public void OnRequestPlayerEntities(long playerId)
        {
            Debug.Log(
                $"[NetworkRequestManager] Sending player entities request for Player ID: {playerId}"
            );
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteShort(NetworkCommands.REQUEST_PLAYER_ENTITIES_C);
            writer.WriteLong(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }
    }
}
