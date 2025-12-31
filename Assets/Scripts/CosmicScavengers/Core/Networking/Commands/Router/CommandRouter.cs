using System.IO;
using System.Linq;
using CosmicScavengers.Core.Networking.Extensions;
using CosmicScavengers.Core.Networking.Responses.Channels;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Router
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

        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to listen for Binary responses.")]
        private BinaryResponseChannel binaryResponseChannel;

        [SerializeField]
        [Tooltip("Channel to listen for Text responses.")]
        private TextResponseChannel textResponseChannel;

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[NetworkRequestManager] ClientConnector reference is missing!");
            }
            if (binaryResponseChannel == null)
            {
                Debug.LogError(
                    "[NetworkRequestManager] BinaryResponseChannel reference is missing!"
                );
            }
            if (textResponseChannel == null)
            {
                Debug.LogError("[NetworkRequestManager] TextResponseChannel reference is missing!");
            }
        }

        void OnEnable()
        {
            clientConnector.OnBinaryMessageReceived += HandleBinaryMessage;
            clientConnector.OnTextMessageReceived += HandleTextMessage;
        }

        void OnDisable()
        {
            clientConnector.OnBinaryMessageReceived -= HandleBinaryMessage;
            clientConnector.OnTextMessageReceived -= HandleTextMessage;
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
            NetworkBinaryCommand command = (NetworkBinaryCommand)commandCode;
            binaryResponseChannel.Raise(command, protobufData);
        }

        private void HandleTextMessage(string rawMessage)
        {
            string[] parts = rawMessage.Split('|');
            if (parts.Length < 1)
            {
                Debug.LogWarning("[NetworkRequestManager] Received malformed text message.");
                return;
            }

            string rawCommand = parts[0];
            NetworkTextCommand command = rawCommand.ToNetworkCommand();
            if (command == NetworkTextCommand.UNKNOWN)
            {
                Debug.LogWarning(
                    $"[NetworkRequestManager] Received unknown text command: {rawCommand}"
                );
                return;
            }
            string[] data = parts.Skip(1).ToArray();
            textResponseChannel.Raise(command, data);
        }
    }
}
