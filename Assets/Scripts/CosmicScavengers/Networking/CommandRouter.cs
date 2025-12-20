using System;
using System.IO;
using System.Linq;
using CosmicScavengers.Networking.Communication;
using CosmicScavengers.Networking.Extensions;
using UnityEngine;

namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Listens for high-level game events and translates them into network requests.
    /// This acts as a bridge between the game logic and the low-level ClientConnector.
    /// </summary>
    public class CommandRouter : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        [Tooltip("The ClientConnector responsible for low-level network communication.")]
        private ClientConnector clientConnector;

        [SerializeField]
        private TextCommandHandlers textCommandHandlers;

        [SerializeField]
        private BinaryCommandHandlers binaryCommandHandlers;

        void Start()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[NetworkRequestManager] ClientConnector reference is missing!");
                return;
            }
            clientConnector.OnBinaryMessageReceived += HandleBinaryMessage;
            clientConnector.OnTextMessageReceived += HandleTextMessage;

            if (binaryCommandHandlers == null)
            {
                Debug.LogError(
                    "[NetworkRequestManager] NetworkCommandHandlers reference is missing!"
                );
            }
        }

        void OnDestroy()
        {
            if (clientConnector != null)
            {
                clientConnector.OnBinaryMessageReceived -= HandleBinaryMessage;
                clientConnector.OnTextMessageReceived -= HandleTextMessage;
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

            short commandCode = reader.ReadShort();
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

            byte[] protobufData = reader.ReadBytes(protobufLength);
            binaryCommandHandlers.Handle(commandCode, protobufData);
        }

        private void HandleTextMessage(string rawMessage)
        {
            string[] parts = rawMessage.Split('|');
            if (parts.Length < 1)
            {
                Debug.LogWarning("[NetworkRequestManager] Received malformed text message.");
                return;
            }

            string command = parts[0];
            string[] data = parts.Skip(1).ToArray();
            textCommandHandlers.Handle(command, data);
        }
    }
}
