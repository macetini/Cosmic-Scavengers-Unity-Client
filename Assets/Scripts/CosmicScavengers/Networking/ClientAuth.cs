using UnityEngine;
using System;

namespace Assets.Scripts.CosmicScavengers.Networking
{
    /// <summary>
    /// Handles user authentication state and communication with the server.
    /// This component manages the player's ID and authentication status.
    /// </summary>
    public class ClientAuth : MonoBehaviour
    {
        [SerializeField]
        protected ClientConnector connector;
        // Fired upon successful login or registration. The GameFlowController subscribes to this.
        public event Action<long> OnAuthenticated;
        protected long playerId = -1; // -1 means not logged in
        protected bool IsAuthenticated => playerId != -1;

        void Start()
        {
            if (connector == null)
            {
                // Ensure dependency is assigned in the Inspector
                Debug.LogError("ClientAuth requires the ClientConnector reference to be assigned in the Inspector.");
                return;
            }
            // Subscribe to the raw message event from the connector
            connector.OnTextMessageReceived += HandleAuthMessage;
        }

        void OnDestroy()
        {
            if (connector != null)
            {
                connector.OnTextMessageReceived -= HandleAuthMessage;
            }
        }

        /// <summary>
        /// Handles incoming messages from the server, focusing only on authentication and status codes.
        /// </summary>
        private void HandleAuthMessage(string rawMessage)
        {
            // Protocol Example: S_CONNECT_OK, S_LOGIN_FAIL|INVALID_CREDENTIALS, S_LOGIN_OK|12345

            string[] parts = rawMessage.Split('|');
            string command = parts[0];

            switch (command)
            {
                case "S_CONNECT_OK":
                    Debug.Log("Server Handshake Complete. Ready for Login/Register.");                    
                    // Optionally trigger a UI state change (e.g., enable the login form)
                    Login("player_1", "secret"); // Auto-login for testing
                    break;

                case "S_REGISTER_OK":
                    Debug.Log("Registration successful. Please log in.");
                    // Notify UI to show a success message
                    break;

                case "S_REGISTER_FAIL":
                    //Debug.LogError($"Registration failed: {parts.ElementAtOrDefault(1) ?? "Unknown error."}");
                    // Notify UI to show the error message
                    break;

                case "S_LOGIN_OK":
                    if (parts.Length > 1 && long.TryParse(parts[1], out long id))
                    {
                        playerId = id;
                        Debug.Log($"Login successful! Player ID: {playerId}. Starting game flow...");
                        OnAuthenticated?.Invoke(playerId); // Signal the GameFlowController to proceed
                    }
                    else
                    {
                        Debug.LogError("Login OK message received but contained invalid Player ID.");
                    }
                    break;

                case "S_LOGIN_FAIL":
                    //Debug.LogError($"Login failed: {parts.ElementAtOrDefault(1) ?? "Invalid Credentials."}");
                    // Notify UI to show the error message
                    break;

                default:
                    // If the message is not an Auth message, it might be a general chat message or an unknown command.
                    Debug.Log($"[ClientAuth] Unhandled server message: {rawMessage}");
                    break;
            }
        }


        // --- Public Methods to send Auth Commands (Triggered by UI) ---

        public void Register(string username, string password)
        {
            if (connector != null && connector.IsConnected)
            {
                connector.SendInput($"C_REGISTER|{username}|{password}");
            }
            else
            {
                Debug.LogError("Cannot register: Client is not connected to the server.");
            }
        }

        public void Login(string username, string password)
        {
            Debug.Log($"Attempting login for user: {username}");
            if (connector != null && connector.IsConnected)
            {
                connector.SendInput($"C_LOGIN|{username}|{password}");
            }
            else
            {
                Debug.LogError("Cannot log in: Client is not connected to the server.");
            }
        }

        /// <summary>
        /// A convenient wrapper to send a game command, ensuring authentication status is checked first.
        /// </summary>
        public void SendAuthenticatedCommand(string command)
        {
            Debug.Log($"[ClientAuth] Sending command: {command}");
            if (IsAuthenticated)
            {
                // This command is passed to the low-level connector
                connector.SendInput(command);
            }
            else
            {
                Debug.LogWarning("Command blocked: Cannot send command because user is not authenticated.");
            }
        }
    }
}