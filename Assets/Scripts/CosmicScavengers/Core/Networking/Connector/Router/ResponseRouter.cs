using System.IO;
using System.Linq;
using CosmicScavengers.Core.Extensions;
using CosmicScavengers.Core.Networking.Commands.Channel.Inbound;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Connector.Router
{
    /// <summary>
    /// Listens for high-level game events and translates them into network requests.
    /// This acts as a bridge between the game logic and the low-level ClientConnector.
    /// </summary>
    public class ResponseRouter : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        [Tooltip("The ClientConnector responsible for low-level network communication.")]
        private ClientConnector clientConnector;

        [Header("Channels Configuration")]
        [Tooltip("Inbound channel for incoming networking messages.")]
        [SerializeField]
        private NetworkingInboundChannel inboundChannel;

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[ResponseRouter] ClientConnector reference is missing!");
            }
            if (inboundChannel == null)
            {
                Debug.LogError("[ResponseRouter] ResponseChannel reference is missing!");
            }
        }

        void OnEnable()
        {
            clientConnector.OnBinaryMessageReceived += HandleBinaryResponseMessage;
            clientConnector.OnTextMessageReceived += HandleTextResponseMessage;
        }

        void OnDisable()
        {
            clientConnector.OnBinaryMessageReceived -= HandleBinaryResponseMessage;
            clientConnector.OnTextMessageReceived -= HandleTextResponseMessage;
        }

        private void HandleBinaryResponseMessage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning("[ResponseRouter] Received empty binary message.");
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
                    $"[ResponseRouter] Invalid frame length ({frameLength} bytes). Expected size: >={MIN_PAYLOAD_SIZE} and <={data.Length - HEADER_SIZE_READ}"
                );
                return;
            }

            int protobufLength = reader.ReadInt();
            // Secondary validation: Check if Lp is also consistent with Frame Length
            // Since FrameLength (Lt) = Lp + Protobuf Data (P), Lp cannot be bigger than Lt.
            if (protobufLength > frameLength - sizeof(int))
            {
                Debug.LogError(
                    $"[ResponseRouter] Corrupt data: Protobuf length ({protobufLength}) exceeds remaining frame payload size."
                );
                return;
            }

            byte[] protobufData = reader.ReadBytes(protobufLength);
            NetworkBinaryCommand command = (NetworkBinaryCommand)commandCode;
            inboundChannel.Raise(command, new InboundData(protobufData, protobufData.Length));
        }

        private void HandleTextResponseMessage(string rawMessage)
        {
            string[] parts = rawMessage.Split('|');
            if (parts.Length < 1)
            {
                Debug.LogWarning("[ResponseRouter] Received malformed text message.");
                return;
            }

            string rawCommand = parts[0];
            NetworkTextCommand command = rawCommand.ToNetworkCommand();
            if (command == NetworkTextCommand.UNKNOWN)
            {
                Debug.LogWarning($"[ResponseRouter] Received unknown text command: {rawCommand}");
                return;
            }
            string[] dataParts = parts.Skip(1).ToArray();
            inboundChannel.Raise(command, new InboundData(dataParts, dataParts.Length));
        }
    }
}
