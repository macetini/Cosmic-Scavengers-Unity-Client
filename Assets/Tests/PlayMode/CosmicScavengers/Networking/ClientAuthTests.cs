using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using CosmicScavengers.Networking;
using Assets.Tests.PlayMode.CosmicScavengers.Networking.Mocks;

namespace Assets.Tests.PlayMode.CosmicScavengers.Networking
{
    [TestFixture]
    public class ClientAuthTests
    {
        //
        // Test Harness for ClientAuth to expose internal state for verification
        //
        private class ClientAuthTestHarness : ClientAuth
        {
            public void SetConnector(ClientConnector connector)
            {
                base.connector = connector;
            }

            // Expose the protected HandleAuthMessage for testing
            public void PublicHandleAuthMessage(string rawMessage)
            {
                //HandleAuthMessage(rawMessage);
            }

            public long PlayerIdForTest => playerId;
            public new bool IsAuthenticated => base.IsAuthenticated;
        }

        private ClientAuthTestHarness clientAuthHarness;
        private ClientConnectorMock clientConnectorMock;
        private GameObject testHost;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            testHost = new GameObject("AuthTestHost");
            // 1. Add the Test Harness component (the CUT)
            clientAuthHarness = testHost.AddComponent<ClientAuthTestHarness>();
            // 2. Add the Mock component (the dependency)
            clientConnectorMock = testHost.AddComponent<ClientConnectorMock>();
            // 3. Link the dependency
            clientAuthHarness.SetConnector(clientConnectorMock);

            // Wait one frame for ClientAuth.Start() to run and subscribe to the connector
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator Teardown()
        {
            Object.Destroy(testHost);
            yield return null;
        }

        // ----------------------------------------------------------------------
        // TEST 2: Failed Registration Flow (Inbound)
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T02_RegisterFail_StaysUnauthenticated()
        {
            // Arrange
            string registerFAILMessage = "S_REGISTER_FAIL|Username unavailable";

            Assert.IsFalse(clientAuthHarness.IsAuthenticated, "Pre-test state check failed.");

            // Act: Inject the failure message
            clientConnectorMock.SimulateMessageReceived(registerFAILMessage);
            yield return null;

            // Assert
            Assert.IsFalse(clientAuthHarness.IsAuthenticated, "ClientAuth state must remain unauthenticated after register failure.");
        }

        // ----------------------------------------------------------------------
        // TEST 3: Login Command Formatting (Outbound)
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T03_Login_SendsCorrectlyFormattedCommand()
        {
            // Arrange
            const string username = "player_1";
            const string password = "secret";
            const string expectedCommand = "C_LOGIN|player_1|secret";

            // Act
            clientAuthHarness.Login(username, password);
            yield return null;

            // Assert: Verify the Mock recorded the exact command
            Assert.AreEqual(expectedCommand, clientConnectorMock.LastSentCommand, "Login command was not formatted correctly.");
        }

        // ----------------------------------------------------------------------
        // TEST 4: Register Command Formatting (Outbound)
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T04_Register_SendsCorrectlyFormattedCommand()
        {
            // Arrange
            const string username = "NewUser";
            const string password = "NewPass456";
            const string expectedCommand = "C_REGISTER|NewUser|NewPass456";

            // Act
            clientAuthHarness.Register(username, password);
            yield return null;

            // Assert: Verify the Mock recorded the exact command
            Assert.AreEqual(expectedCommand, clientConnectorMock.LastSentCommand, "Register command was not formatted correctly.");
        }

        // ----------------------------------------------------------------------
    }
}