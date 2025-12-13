using UnityEngine;
using System;
using System.IO;
using System.Text;
using CosmicScavengers.Networking.Extensions;
using CosmicScavengers.Networking.Event.Channels;
using System.Collections.Generic;

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
                throw new Exception("[NetworkRequestManager] ClientConnector dependency is not assigned.");
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
                    //Debug.Log("[NetworkRequestManager] Received world state response from server: " + worldData);
                    //getWorldDataEventChannel.Raise(worldData);
                    break;
                case NetworkCommands.REQUEST_PLAYER_ENTITIES_S:
                    payloadLength = reader.ReadInt();
                    byte[] playerEntitiesData = reader.ReadBytes(payloadLength);
                    List<EntityData> playerEntities = ParsePlayerEntities(playerEntitiesData);
                    Debug.Log("[NetworkRequestManager] Received player entities response from server. Count: " + playerEntities.Count);
                    entitiesUpdateEventChannel.Raise(playerEntities);
                    break;
                default:
                    Debug.LogWarning("[NetworkRequestManager] Unhandled command received: " + command);
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
            Debug.Log($"[NetworkRequestManager] Sending world state request for Player ID: {playerId}");
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteShort(NetworkCommands.REQUEST_WORLD_DATA_C);
            writer.WriteLong(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }

        public void OnRequestPlayerEntities(long playerId)
        {
            Debug.Log($"[NetworkRequestManager] Sending player entities request for Player ID: {playerId}");
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteShort(NetworkCommands.REQUEST_PLAYER_ENTITIES_C);
            writer.WriteLong(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }

        private static List<EntityData> ParsePlayerEntities(byte[] playerEntitiesData)
        {
            List<EntityData> playerEntities;

            // Create a new MemoryStream and BinaryReader specifically for the payload
            using (MemoryStream payloadStream = new(playerEntitiesData))
            using (BinaryReader payloadReader = new(payloadStream))
            {
                // Read the number of player entities (4 bytes, Little Endian)
                int entityCount = payloadReader.ReadInt();
                playerEntities = new List<EntityData>(entityCount);
                for (int i = 0; i < entityCount; i++)
                {
                    EntityData entity = new()
                    {
                        Id = payloadReader.ReadLong(),
                        //PlayerId = payloadReader.ReadInt64BE(),
                        //WorldId = payloadReader.ReadInt64BE(),

                        //int entityTypeLength = payloadReader.ReadInt32BE();
                        //byte[] entityTypeBytes = payloadReader.ReadBytes(entityTypeLength);
                        //entity.Type = Encoding.UTF8.GetString(entityTypeBytes);

                        ChunkX = payloadReader.ReadInt(),
                        ChunkY = payloadReader.ReadInt(),

                        PosX = payloadReader.ReadFloat(),
                        PosY = payloadReader.ReadFloat(),

                        Health = payloadReader.ReadInt()
                    };
                    //Debug.Log($"[NetworkRequestManager] Parsed EntityData: ID={entity.Id}, PlayerID={entity.PlayerId}, WorldID={entity.WorldId}, Chunk=({entity.ChunkX},{entity.ChunkY}), Position=({entity.PosX},{entity.PosY}), Health={entity.Health}");
                    playerEntities.Add(entity);
                }
            }

            return playerEntities;
        }
    }
}