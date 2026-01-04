using CosmicScavengers.Networking.Commands.Meta;
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

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[MessageDispatcher] ClientConnector reference is missing!");
            }
        }

        private void HandleDispatchedMessage(CommandType command, object data)
        {
            // Implement message handling logic based on command type
            Debug.Log($"Dispatched Message - Command: {command}, Data: {data}");
        }
    }
}
