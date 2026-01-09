using CosmicScavengers.Core.Networking.Commands.Channel.Outbound;
using CosmicScavengers.Core.Networking.Commands.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Connector.Dispatcher
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
        private NetworkingOutboundChannel outboundChannel;

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[MessageDispatcher] ClientConnector reference is missing!");
            }
            if (outboundChannel == null)
            {
                Debug.LogError(
                    "[MessageDispatcher] NetworkingOutboundChannel reference is missing!"
                );
            }
        }

        void OnEnable()
        {
            outboundChannel.AddListener(HandleRequest);
        }

        void OnDisable()
        {
            outboundChannel.RemoveListener(HandleRequest);
        }

        private void HandleRequest(BaseNetworkCommand command, OutboundData data)
        {
            byte[] buffer = data.RawBytes;
            int bufferLength = data.DataLength;
            if (buffer == null || buffer.Length == 0 || bufferLength <= 0)
            {
                Debug.LogWarning(
                    "[MessageDispatcher] Received empty or null binary data for command: "
                        + command.ToString()
                );
                return;
            }
            clientConnector.DispatchMessage(buffer, bufferLength, command.Type);
        }
    }
}
