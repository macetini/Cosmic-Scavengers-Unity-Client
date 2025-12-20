using CosmicScavengers.Networking.Event.Channels;
using UnityEngine;

namespace CosmicScavengers.Networking
{
    public class MessageDispatcher : MonoBehaviour
    {
        [Header("Dependencies")]
        [Tooltip("Reference to the ClientConnector for sending messages.")]
        [SerializeField]
        private ClientConnector clientConnector;

        [Header("Command Channels")]
        [Tooltip("Channel for dispatching text commands.")]
        [SerializeField]
        private TextCommandChannel textCommandChannel;

        [SerializeField]
        [Tooltip("Channel for dispatching binary commands.")]
        private BinaryCommandChannel binaryCommandChannel;

        void Start()
        {
            if (clientConnector == null)
            {
                Debug.LogError("ClientConnector reference is missing in MessageDispatcher.");
            }
            if (textCommandChannel == null)
            {
                Debug.LogError("TextCommandChannel reference is missing in MessageDispatcher.");
            }

            textCommandChannel.AddListener(DispatchTextMessage);
            binaryCommandChannel.AddListener(DispatchBinaryMessage);
        }

        void OnDestroy()
        {
            textCommandChannel.RemoveListener(DispatchTextMessage);
            binaryCommandChannel.RemoveListener(DispatchBinaryMessage);
        }

        private void DispatchTextMessage(string command)
        {
            if (clientConnector != null && clientConnector.IsConnected)
            {
                clientConnector.DispatchTextMessage(command);
            }
            else
            {
                Debug.LogWarning("Cannot send text command. ClientConnector is not connected.");
            }
        }

        private void DispatchBinaryMessage(byte[] data)
        {
            if (clientConnector != null && clientConnector.IsConnected)
            {
                clientConnector.DispatchBinaryMessage(data);
            }
            else
            {
                Debug.LogWarning("Cannot send binary command. ClientConnector is not connected.");
            }
        }
    }
}
