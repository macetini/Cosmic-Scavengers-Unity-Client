using System;
using CosmicScavengers.Networking.Channel;
using CosmicScavengers.Networking.Channel.Data;
using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Commands.Data.Binary;
using CosmicScavengers.Networking.Commands.Data.Meta;
using CosmicScavengers.Networking.Commands.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Connector.Dispatcher
{
    /// <summary>
    /// Dispatches incoming messages from the ClientConnector to the appropriate handlers.
    /// </summary>
    public class MessageDispatcher : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        [Tooltip("The ClientConnector responsible for low-level network communication.")]
        private ClientConnector clientConnector;

        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Inbound channel for incoming requests.")]
        private NetworkingChannel networkingChannel;

        private const string COMMAND_DELIMITER = "|";

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[MessageDispatcher] ClientConnector reference is missing!");
            }
            if (networkingChannel == null)
            {
                Debug.LogError("[MessageDispatcher] NetworkingChannel reference is missing!");
            }
        }

        void OnEnable()
        {
            networkingChannel.AddListener(RouteNetworkingRequest);
        }

        void OnDisable()
        {
            networkingChannel.RemoveListener(RouteNetworkingRequest);
        }

        private void RouteNetworkingRequest(BaseNetworkCommand command, NetworkingChannelData data)
        {
            switch (command.Type)
            {
                case CommandType.BINARY:
                    HandleBinaryRequest(command.BinaryCommand, data.RawBytes);
                    break;
                case CommandType.TEXT:
                    HandleTextRequest(command.TextCommand, data.TextParts);
                    break;
                case CommandType.UNKNOWN:
                    Debug.LogWarning(
                        $"[MessageDispatcher] - Received Unknown Command Type: {command.Type}"
                    );
                    break;
                default:
                    throw new Exception($"Impossible Command Type: {command.Type}");
            }
        }

        private void HandleBinaryRequest(NetworkBinaryCommand binaryCommand, byte[] data)
        {
            throw new NotImplementedException();
        }

        private void HandleTextRequest(NetworkTextCommand textCommand, string[] data)
        {
            string fullMessage =
                (data == null || data.Length == 0)
                    ? textCommand.ToString()
                    : $"{textCommand}|{string.Join(COMMAND_DELIMITER, data)}";

            clientConnector.DispatchTextMessage(fullMessage);
        }
    }
}
