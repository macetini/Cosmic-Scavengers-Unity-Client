using UnityEngine;

namespace CosmicScavengers.Networking.MessageHandling
{
    public class ClientMessageProcessor
    {
        // Delegate definition: signals successful login and provides the Player ID
        public delegate void AuthenticationSuccessHandler(long playerId);
        // Event declaration
        public event AuthenticationSuccessHandler OnAuthenticated;
        private long playerId = -1; // -1 indicates unauthenticated state

        public void ProcessServerMessage(string rawMessage)
        {
            // Example logic: Server sends "S_AUTH_OK|12345"
            string[] parts = rawMessage.Split('|');
            string command = parts[0];

            if (command == "S_AUTH_OK")
            {
                if (parts.Length > 1 && long.TryParse(parts[1], out long id))
                {
                    // This is the successful authentication logic
                    playerId = id;
                    Debug.Log($"Login successful! Player ID: {playerId}. Starting game flow...");

                    // DISPATCH THE EVENT
                    OnAuthenticated?.Invoke(playerId); // Signal the GameFlowController to proceed
                }
                else
                {
                    Debug.LogError("Authentication response missing Player ID.");
                }
            }
            else
            {
                Debug.LogWarning($"Unhandled server message: {rawMessage}");
            }            
        }
    }
}