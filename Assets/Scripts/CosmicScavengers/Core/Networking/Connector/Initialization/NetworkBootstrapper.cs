using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Connector.Initialization
{
    public class NetworkBootstrapper : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        private RequestChannel requestChannel;

        [Header("Dependencies")]
        [SerializeField]
        [Tooltip("The ClientConnector responsible for managing the network connection.")]
        private ClientConnector clientConnector;

        private bool triggeredInitialConnection = false;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("[NetworkBootstrapper] RequestChannel is not assigned.");
            }
            if (clientConnector == null)
            {
                Debug.LogError("[NetworkBootstrapper] ClientConnector is not assigned.");
            }
        }

        void OnEnable()
        {
            clientConnector.OnConnected += ConnectionEstablished;
        }

        void OnDisable()
        {
            clientConnector.OnConnected -= ConnectionEstablished;
        }

        private void ConnectionEstablished()
        {
            TriggerInitialConnection();
        }

        void Start()
        {
            if (!triggeredInitialConnection && clientConnector.IsConnected)
            {
                TriggerInitialConnection();
            }
        }

        private void TriggerInitialConnection()
        {
            Debug.Log("[NetworkBootstrapper] Game started. Raising C_CONNECT intent.");
            triggeredInitialConnection = true;
            requestChannel.Raise(NetworkTextCommand.C_CONNECT);
        }
    }
}
