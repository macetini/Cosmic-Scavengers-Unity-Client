using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using Assets.Scripts.CosmicScavengers.Networking;
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
                HandleAuthMessage(rawMessage);
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
        // TEST 1: Successful Login Flow (Inbound)
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T01_LoginOK_UpdatesStateAndFiresEvent()
        {
            // Arrange
            const long expectedPlayerId = 1001;
            string loginOKMessage = $"S_LOGIN_OK|{expectedPlayerId}";
            bool eventFired = false;
            long receivedId = -1;

            clientAuthHarness.OnAuthenticated += (id) =>
            {
                eventFired = true;
                receivedId = id;
            };

            clientConnectorMock.TestDispatchMessage += clientAuthHarness.PublicHandleAuthMessage;

            // Act: Inject the successful login message            
            Assert.IsFalse(clientAuthHarness.IsAuthenticated, "Pre-test state check failed.");
            clientConnectorMock.SimulateMessageReceived(loginOKMessage);
            yield return null;

            // Assert
            Assert.IsTrue(clientAuthHarness.IsAuthenticated, "ClientAuth state must be authenticated.");
            Assert.AreEqual(expectedPlayerId, clientAuthHarness.PlayerIdForTest, "Player ID must match the received ID.");
            Assert.IsTrue(eventFired, "OnAuthenticated event must have fired.");
            Assert.AreEqual(expectedPlayerId, receivedId, "Event payload must contain the correct player ID.");
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
        // TEST 5: SendAuthenticatedCommand - Authenticated
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T05_SendAuthenticatedCommand_WhenAuthenticated_SendsCommand()
        {
            // Ensure state is authenticated from the start
            clientConnectorMock.TestDispatchMessage += clientAuthHarness.PublicHandleAuthMessage;
            clientConnectorMock.SimulateMessageReceived("S_LOGIN_OK|12345");
            yield return null;

            const string gameCommand = "C_MOVE|10|20";

            // Act
            clientAuthHarness.SendAuthenticatedCommand(gameCommand);
            yield return null;

            // Assert
            Assert.AreEqual(gameCommand, clientConnectorMock.LastSentCommand, "Authenticated command should have been sent.");
            Assert.IsTrue(clientAuthHarness.IsAuthenticated, "Client should remain authenticated.");
        }

        // ----------------------------------------------------------------------
        // TEST 6: SendAuthenticatedCommand - Unauthenticated (Negative Path)
        // ----------------------------------------------------------------------
        [UnityTest]
        public IEnumerator T06_SendAuthenticatedCommand_WhenUnauthenticated_CommandIsBlocked()
        {
            // Arrange
            // Ensure state is unauthenticated from the start
            clientConnectorMock.ResetState();
            Assert.IsFalse(clientAuthHarness.IsAuthenticated, "Pre-test state must be unauthenticated.");

            const string gameCommand = "C_MOVE|10|20";
            const string initialCommand = "C_LOGIN|dummy|pass";

            // Act 1: Send a command that should succeed (e.g., login) to set a known initial state for LastSentCommand
            // The client is NOT authenticated, so this call to Login only formats the command.
            clientAuthHarness.Login("dummy", "pass");
            yield return null;

            // Assert 1: Check the state after the dummy command
            Assert.AreEqual(initialCommand, clientConnectorMock.LastSentCommand, "Mock should have recorded the login command.");

            // Act 2: Try to send an authenticated command while still unauthenticated
            clientAuthHarness.SendAuthenticatedCommand(gameCommand);
            yield return null;

            // Assert 2: The LastSentCommand on the Mock MUST NOT have changed, proving the command was blocked.
            Assert.AreEqual(initialCommand, clientConnectorMock.LastSentCommand, "Unauthenticated command must be blocked, keeping the previous command in the mock.");
            Assert.IsFalse(clientAuthHarness.IsAuthenticated, "Client should still be unauthenticated.");
        }
    }
}