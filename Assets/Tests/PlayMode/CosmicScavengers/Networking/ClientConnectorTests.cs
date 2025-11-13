using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools; 
using System.Collections;
using Assets.Tests.PlayMode.CosmicScavengers.Networking.Stubs; 

namespace Assets.Tests.PlayMode.CosmicScavengers.Networking 
{
    [TestFixture]
    public class ClientConnectorTests
    {
        private ClientConnectorStub connector;
        private GameObject testObject;

        [UnitySetUp]
        // Runs before every test method in Play Mode
        public IEnumerator Setup()
        {
            // Create a GameObject to host the MonoBehaviour component
            testObject = new GameObject("TestConnectorHost");
            
            // Add the Testable Stub component
            connector = testObject.AddComponent<ClientConnectorStub>();
            
            // Wait one frame for the Unity lifecycle methods (Awake/Start) to run
            yield return null; 
        }

        [UnityTearDown]
        // Runs after every test method in Play Mode
        public IEnumerator Teardown()
        {
            // Clean up the created GameObject
            Object.Destroy(testObject);
            yield return null;
        }

        // ----------------------------------------------------------------------
        // TEST 1: Message Dispatch
        // Verifies a simulated network message is processed by Update() and fires the Action.
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T01_SimulatedMessage_FiresActionOnMainThread()
        {
            // Arrange
            const string expectedMessage = "TEST_MESSAGE|123";
            string receivedMessage = null;

            // Subscribe to the public Action
            connector.TestDispatchMessage += (msg) => 
            {
                receivedMessage = msg;
            };

            // Act - Simulate the network thread adding a message to the internal queue
            connector.SimulateMessageReceived(expectedMessage);
            
            // Wait for the next Update cycle to run
            yield return null;

            // Assert 
            Assert.IsNotNull(receivedMessage, "Message must be processed and received after the Update cycle.");
            Assert.AreEqual(expectedMessage, receivedMessage, "Received message content must match the expected content.");
        }
        
        // ----------------------------------------------------------------------
        // TEST 2: No Message
        // Verifies that the Action is not invoked if the queue is empty.
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T02_EmptyQueue_DoesNotFireAction()
        {
            // Arrange
            bool actionFired = false;
            connector.TestDispatchMessage += (msg) => { actionFired = true; };

            // Act - Do nothing

            // Wait for the next Update cycle
            yield return null;

            // Assert
            Assert.IsFalse(actionFired, "No action should fire if the message queue is empty.");
        }
    }
}