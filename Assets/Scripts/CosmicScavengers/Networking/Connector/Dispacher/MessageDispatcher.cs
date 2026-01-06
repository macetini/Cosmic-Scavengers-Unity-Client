using CosmicScavengers.Networking.Channel.Data.Request;
using CosmicScavengers.Networking.Channel.Request;
using CosmicScavengers.Networking.Commands;
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
        private NetworkingRequestChannel networkingChannel;

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
            networkingChannel.AddListener(HandleRequest);
        }

        void OnDisable()
        {
            networkingChannel.RemoveListener(HandleRequest);
        }

        private void HandleRequest(BaseNetworkCommand command, RequestData data)
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
