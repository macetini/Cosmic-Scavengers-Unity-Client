using UnityEngine;
using System;
using System.IO;
using CosmicScavengers.Core.Models;
using System.Text;
using CosmicScavengers.Networking.Extensions;
using CosmicScavengers.Networking.Event.Channels;

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
            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);
            short command = reader.ReadInt16BE();
            switch (command)
            {
                case NetworkCommands.REQUEST_WORLD_DATA:
                    int payloadLength = reader.ReadInt32BE();
                    byte[] worldStateData = reader.ReadBytes(payloadLength);
                    WorldData worldData = ParseWorldState(worldStateData);
                    Debug.Log("[NetworkRequestManager] Received world state response from server: " + worldData);
                    getWorldDataEventChannel.Raise(worldData.MapSeed);
                    break;
                case NetworkCommands.REQUEST_PLAYER_ENTITIES:                    
                    Debug.Log("[NetworkRequestManager] Received player entities response from server.");
                    break;
                default:
                    Debug.LogWarning("[NetworkRequestManager] Unhandled command received: " + command);
                    break;
            }
            //Debug.Log("[NetworkRequestManager] Received binary message of length: " + obj.Length);
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

        private WorldData ParseWorldState(byte[] worldStateData)
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
    }
}