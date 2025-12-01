using UnityEngine;
using System;
using System.IO;
using CosmicScavengers.Core.Models;
using System.Text;
using CosmicScavengers.Networking.Ext;

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
        private ClientConnector clientConnector;

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
                case NetworkCommands.REQUEST_WORLD_STATE:
                    int payloadLength = reader.ReadInt32BE();
                    byte[] worldStateData = reader.ReadBytes(payloadLength);
                    WorldState worldState = ParseWorldState(worldStateData);
                    Debug.Log("[NetworkRequestManager] Received world state response from server: " + worldState);
                    break;
                // Handle different command types here
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
            writer.WriteInt16BE(NetworkCommands.REQUEST_WORLD_STATE);
            writer.WriteInt64BE(playerId);
            
            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }

        private WorldState ParseWorldState(byte[] worldStateData)
        {
            WorldState state = new();

            // Create a new MemoryStream and BinaryReader specifically for the payload
            using (MemoryStream payloadStream = new(worldStateData))
            using (BinaryReader payloadReader = new(payloadStream))
            {
                // 1. Read World ID (8 bytes, Little Endian)
                state.WorldId = payloadReader.ReadInt64BE();

                // 2. Read World Name Length (4 bytes, Little Endian)
                int nameLength = payloadReader.ReadInt32BE();

                // 3. Read World Name (variable length, using UTF-8)
                // Read the specified number of bytes
                byte[] nameBytes = payloadReader.ReadBytes(nameLength);
                // Convert bytes to string using the agreed-upon encoding (usually UTF-8)
                state.WorldName = Encoding.UTF8.GetString(nameBytes);

                // 4. Read Map Seed (4 bytes, Little Endian)
                state.MapSeed = payloadReader.ReadInt64BE();

                // 5. Read Sector Size Units (4 bytes, Little Endian)
                state.SectorSizeUnits = payloadReader.ReadInt32BE();
            }

            return state;
        }
    }
}