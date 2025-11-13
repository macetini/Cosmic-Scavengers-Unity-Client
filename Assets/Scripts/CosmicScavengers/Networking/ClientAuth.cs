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
        private ClientConnector connector;
        
        // Fired upon successful login or registration. The GameFlowController subscribes to this.
        public event Action<long> OnAuthenticated;
        
        // --- State ---
        private long playerId = -1; // -1 means not logged in
        public bool IsAuthenticated => playerId != -1;
        
        void Start()
        {
            if (connector == null)
            {
                // Ensure dependency is assigned in the Inspector
                Debug.LogError("ClientAuth requires the ClientConnector reference to be assigned in the Inspector.");
                return;
            }
            
            // Subscribe to the raw message event from the connector
            connector.OnMessageReceived += HandleAuthMessage;
        }

        void OnDestroy()
        {
            if (connector != null)
            {
                connector.OnMessageReceived -= HandleAuthMessage;
            }
        }

        /// <summary>
        /// Handles incoming messages from the server, focusing only on authentication and status codes.
        /// </summary>
        private void HandleAuthMessage(string rawMessage)
        {
            string[] parts = rawMessage.Split('|');
            if (parts.Length == 0) return;
            string commandCode = parts[0];
            
            switch (commandCode)
            {
                case "S_CONNECT_OK":
                    // Connection confirmed, but we wait for user input (UI buttons) to authenticate.
                    Debug.Log("Connection confirmed by server. Waiting for user to log in or register.");
                    Login("player_1", "secret");
                    break;
                    
                case "S_REGISTER_OK":
                case "S_LOGIN_OK":
                    if (parts.Length < 2) break;
                    playerId = long.Parse(parts[1]);
                    Debug.Log($"Authentication SUCCESS. Player ID: {playerId}.");
                    
                    // Fire the event to notify the GameFlowController
                    OnAuthenticated?.Invoke(playerId);
                    break;
                    
                case "S_REGISTER_FAIL":
                    if (parts.Length < 2) break;
                    Debug.LogWarning($"Registration FAILED: {parts[1]}. Please try a different username or login.");
                    break;

                case "S_LOGIN_FAIL":
                    if (parts.Length < 2) break;
                    Debug.LogError($"Login FAILED: {parts[1]}. Please check credentials.");
                    playerId = -1;
                    break;
                    
                case "S_AUTH_REQUIRED":
                    Debug.LogError("Server rejected game command: Authentication is required.");
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