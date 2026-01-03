using System.ComponentModel.Design;
using CosmicScavengers.Core.Networking.Commands.Meta;
using CosmicScavengers.Core.Networking.Connector.Dispatcher.Channel;
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

        [Header("Channels Configuration")]
        [SerializeField]
        private MessageDispatchChannel messageDispatchChannel;

        void Awake()
        {
            if (clientConnector == null)
            {
                Debug.LogError("[MessageDispatcher] ClientConnector reference is missing!");
            }
            if (messageDispatchChannel == null)
            {
                //Debug.LogError("[MessageDispatcher] MessageDispatchChannel reference is missing!");
            }
        }

        void OnEnable()
        {
            //messageDispatchChannel.AddListener(HandleDispatchedMessage);
        }

        void OnDisable()
        {
            //messageDispatchChannel.RemoveListener(HandleDispatchedMessage);
        }

        private void HandleDispatchedMessage(CommandType command, object data)
        {
            // Implement message handling logic based on command type
            Debug.Log($"Dispatched Message - Command: {command}, Data: {data}");
        }
    }
}
