using CosmicScavengers.Networking;
using System.Collections.Generic;
using System;

namespace Assets.Tests.PlayMode.CosmicScavengers.Networking.Mocks 
{
    public class ClientConnectorMock : ClientConnector
    {
        // --- MOCK PROPERTY: Records the last sent command for verification ---
        public string LastSentCommand { get; private set; } = string.Empty;

        // --- MOCK FUNCTIONALITY: Reset the mock state ---
        public void ResetState()
        {
            LastSentCommand = string.Empty;
        }

        // --- MOCK FUNCTIONALITY: Simulate Inbound Messages ---
        // We need a public Action that ClientAuthTestHarness can subscribe to 
        // and redirect to ClientAuth.HandleAuthMessage.
        public Action<string> TestDispatchMessage; 
        
        private readonly Queue<string> testIncomingMessages = new();

        public void SimulateMessageReceived(string message)
        {
            testIncomingMessages.Enqueue(message);
        }
        
        /// <summary>
        /// Runs in the Unity Update loop to dispatch simulated messages.
        /// </summary>
        void Update()
        {
            while (testIncomingMessages.Count > 0)
            {
                string message = testIncomingMessages.Dequeue();
                                
                TestDispatchMessage?.Invoke(message); 
            }
        }
    }
}