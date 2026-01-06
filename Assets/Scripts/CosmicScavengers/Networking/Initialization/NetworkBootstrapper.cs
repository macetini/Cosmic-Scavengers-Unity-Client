using CosmicScavengers.Networking.Channel;
using CosmicScavengers.Networking.Commands.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Initialization
{
    public class NetworkBootstrapper : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        private RequestChannel requestChannel;

        [Header("Settings")]
        [SerializeField]
        private bool autoConnectOnStart = true;

        [SerializeField]
        private float delayBeforeConnect = 0.5f;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("[NetworkBootstrapper] RequestChannel is not assigned.");
            }
        }

        void Start()
        {
            if (autoConnectOnStart)
            {
                Invoke(nameof(TriggerInitialConnection), delayBeforeConnect);
            }
        }

        private void TriggerInitialConnection()
        {
            Debug.Log("[NetworkBootstrapper] Game started. Raising C_CONNECT intent.");
            requestChannel.Raise(NetworkTextCommand.C_CONNECT);
        }
    }
}
