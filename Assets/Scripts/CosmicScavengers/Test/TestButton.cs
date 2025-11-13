using Assets.Scripts.CosmicScavengers.Networking;
using UnityEngine;

namespace Assets.Scripts.CosmicScavengers.Test
{
    public class TestButton : MonoBehaviour
    {
        private ClientConnector connector;

        void Start()
        {
            // Find the ClientConnector script in the scene
            connector = FindAnyObjectByType<ClientConnector>();
            if (connector == null)
            {
                Debug.LogError("ClientConnector script not found in the scene.");
            }
        }

        // This method is called when the UI Button is pressed
        public void OnSendTestMessage()
        {
            if (connector != null)
            {
                // Send a simple, identifiable message to the server
                connector.SendInput("TEST_MESSAGE: Hello from Unity!");
            }
        }
    }
}