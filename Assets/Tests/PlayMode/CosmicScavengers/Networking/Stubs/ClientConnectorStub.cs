using System;
using System.Collections.Generic;
using CosmicScavengers.Networking.Connector;

namespace Assets.Tests.PlayMode.CosmicScavengers.Networking.Stubs
{
    /// <summary>
    /// A Stub component that mimics the message dispatch behavior of ClientConnector
    /// by providing a mechanism to inject simulated network messages for testing.
    /// </summary>
    public class ClientConnectorStub : ClientConnector
    {
        // Local queue to hold messages before they are "processed" by the main thread (Update)
        private readonly Queue<string> testIncomingMessages = new();

        // Public Action that replaces the protected 'OnMessageReceived' event
        public Action<string> TestDispatchMessage;

        /// <summary>
        /// Simulates the network thread receiving a message and adding it to the queue.
        /// </summary>
        public void SimulateMessageReceived(string message)
        {
            testIncomingMessages.Enqueue(message);
        }

        /// <summary>
        /// Overrides the Update method to simulate the main thread polling the queue
        /// and firing the dispatch action.
        /// </summary>
        void Update()
        {
            // Process the messages one by one during the Update cycle
            while (testIncomingMessages.Count > 0)
            {
                string message = testIncomingMessages.Dequeue();
                // Invoke the Action, simulating the event firing on the main thread
                TestDispatchMessage?.Invoke(message);
            }
        }
    }
}
