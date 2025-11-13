using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.CosmicScavengers.Networking; // Adjust namespace as necessary

namespace Assets.Scripts.CosmicScavengers.Test
{
    public class AuthenticationUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public InputField usernameInput;
        public InputField passwordInput;
        public Text statusText;
        private ClientConnector connector;

        void Start()
        {
            // Find the single instance of the connector
            connector = FindFirstObjectByType<ClientConnector>();
            if (connector == null)
            {
                statusText.text = "Error: ClientConnector not found.";
            }
        }

        // --- P_REGISTER Command ---
        public void OnClickRegister()
        {
            if (connector == null || !connector.IsConnected)
            {
                statusText.text = "Error: Not connected to server.";
                return;
            }

            string username = usernameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                statusText.text = "Username and Password are required.";
                return;
            }

            // PROTOCOL: P_REGISTER|Username:USER|Password:PASS
            string command = $"P_REGISTER|Username:{username}|Password:{password}";
            connector.SendInput(command);
            statusText.text = "Sending registration...";
        }

        // --- P_LOGIN Command (The one you need for player_1) ---
        public void OnClickLogin()
        {
            if (connector == null || !connector.IsConnected)
            {
                statusText.text = "Error: Not connected to server.";
                return;
            }

            string username = usernameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                statusText.text = "Username and Password are required.";
                return;
            }

            // PROTOCOL: P_LOGIN|Username:USER|Password:PASS
            string command = $"P_LOGIN|Username:{username}|Password:{password}";
            connector.SendInput(command);
            statusText.text = "Attempting login...";
        }
    }
}