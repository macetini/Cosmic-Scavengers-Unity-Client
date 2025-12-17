using System;
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

            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);

            short command = reader.ReadShort();
            int frameLength = reader.ReadInt();

            const int HEADER_SIZE_READ = sizeof(short) + sizeof(int); // 2 (Command) + 4 (FrameLength) = 6 bytes
            const int MIN_PAYLOAD_SIZE = sizeof(int); // Minimum payload size to at least read the Protobuf length (Lp)

            // Must not be negative or less than the minimum structure size (Lp).
            // Must not require reading past the end of the total received buffer.
            if (frameLength < MIN_PAYLOAD_SIZE || frameLength > data.Length - HEADER_SIZE_READ)
            {
                Debug.LogError(
                    $"[NetworkRequestManager] Invalid frame length ({frameLength} bytes). Expected size: >={MIN_PAYLOAD_SIZE} and <={data.Length - HEADER_SIZE_READ}"
                );
                return;
            }

            int protobufLength = reader.ReadInt();
            // Secondary validation: Check if Lp is also consistent with Frame Length
            // Since FrameLength (Lt) = Lp + Protobuf Data (P), Lp cannot be bigger than Lt.
            if (protobufLength > frameLength - sizeof(int))
            {
                Debug.LogError(
                    $"[NetworkRequestManager] Corrupt data: Protobuf length ({protobufLength}) exceeds remaining frame payload size."
                );
                return;
            }

            switch (command)
            {
                case NetworkCommands.REQUEST_WORLD_DATA_S:
                    byte[] worldDataBytes = reader.ReadBytes(protobufLength);
                    WorldClientDataHandler.Handle(worldDataBytes);
                    //getWorldDataEventChannel.Raise(worldData);
                    break;
                case NetworkCommands.REQUEST_PLAYER_ENTITIES_S:
                    byte[] playerEntitiesBytes = reader.ReadBytes(protobufLength);
                    PlayerEntitiesDataHandler.Handle(playerEntitiesBytes);
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
