using UnityEngine;
using System;
using System.IO;
using CosmicScavengers.Core.Models;
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
            short command = reader.ReadInt16BE();
            switch (command)
            {
                case NetworkCommands.REQUEST_WORLD_DATA:
                    payloadLength = reader.ReadInt32BE();
                    byte[] worldStateData = reader.ReadBytes(payloadLength);
                    WorldData worldData = ParseWorldState(worldStateData);
                    Debug.Log("[NetworkRequestManager] Received world state response from server: " + worldData);
                    getWorldDataEventChannel.Raise(worldData);
                    break;
                case NetworkCommands.REQUEST_PLAYER_ENTITIES:
                    payloadLength = reader.ReadInt32BE();
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
            writer.WriteInt16BE(NetworkCommands.REQUEST_WORLD_DATA);
            writer.WriteInt64BE(playerId);

            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }

        private static WorldData ParseWorldState(byte[] worldStateData)
        {
            WorldData worldData = new();

            // Create a new MemoryStream and BinaryReader specifically for the payload
            using (MemoryStream payloadStream = new(worldStateData))
            using (BinaryReader payloadReader = new(payloadStream))
            {
                // 1. Read World ID (8 bytes, Little Endian)
                worldData.WorldId = payloadReader.ReadInt64BE();
                // 2. Read World Name Length (4 bytes, Little Endian)
                int nameLength = payloadReader.ReadInt32BE();

                // 3. Read World Name (variable length, using UTF-8)
                // Read the specified number of bytes
                byte[] nameBytes = payloadReader.ReadBytes(nameLength);
                // Convert bytes to string using the agreed-upon encoding (usually UTF-8)
                worldData.WorldName = Encoding.UTF8.GetString(nameBytes);

                // 4. Read Map Seed (4 bytes, Little Endian)
                worldData.MapSeed = payloadReader.ReadInt64BE();
                // 5. Read Sector Size Units (4 bytes, Little Endian)
                worldData.SectorSizeUnits = payloadReader.ReadInt32BE();
            }

            return worldData;
        }

        public void OnRequestPlayerEntities(long playerId)
        {
            Debug.Log($"[NetworkRequestManager] Sending player entities request for Player ID: {playerId}");
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.WriteInt16BE(NetworkCommands.REQUEST_PLAYER_ENTITIES);
            writer.WriteInt64BE(playerId);

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