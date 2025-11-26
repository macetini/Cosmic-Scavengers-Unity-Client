using UnityEngine;
using CosmicScavengers.Networking; // Import the namespace where ClientAuth lives
using UnityEngine.SceneManagement; // For loading the main game scene

namespace CosmicScavengers.Core
{
    /// <summary>
    /// The master controller for game state. It waits for successful authentication 
    /// and then triggers the transition to the main game scene or world loading.
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        [Tooltip("The ClientAuth component responsible for handling authentication.")]
        [SerializeField]
        private ClientAuth clientAuth;

        [Tooltip("The name of the scene to load after successful authentication.")]
        public string mainGameSceneName = "MainGameWorld";
        private long localPlayerId = -1;
        private bool isGameFlowStarted = false;

        void Start()
        {
            // 1. Dependency Check
            if (clientAuth == null)
            {
                // This error means the ClientAuth component needs to be linked in the Unity Inspector.
                Debug.LogError("[GameFlowController] ClientAuth reference is missing. Cannot subscribe to events.");
                return;
            }

            // 2. Subscription
            clientAuth.OnAuthenticated += StartGameSession;
            Debug.Log("[GameFlowController] Subscribed to ClientAuth.OnAuthenticated. Awaiting login confirmation...");
        }

        void OnDestroy()
        {
            // Ensure the subscription is cleaned up when the GameObject is destroyed
            if (clientAuth != null)
            {
                clientAuth.OnAuthenticated -= StartGameSession;
            }
        }

        /// <summary>
        /// Triggered directly by the ClientAuth component after a successful S_LOGIN_OK.
        /// This is the entry point into the actual game.
        /// </summary>
        /// <param name="playerId">The ID of the successfully authenticated player.</param>
        private void StartGameSession(long playerId)
        {
            clientAuth.OnAuthenticated -= StartGameSession;

            if (isGameFlowStarted)
            {
                Debug.LogWarning("[GameFlowController] StartGameSession called multiple times. Ignoring subsequent calls.");
                return;
            }

            isGameFlowStarted = true;
            localPlayerId = playerId;

            Debug.Log($"[GameFlow] Login confirmed with ID: {playerId}. Beginning asset loading and scene transition.");

            // --- Game Initialization Steps ---

            // 1. Store the Player ID (e.g., in a static GameState or PlayerDataService)
            // GameState.LocalPlayerID = playerId; 

            // 2. Request initial world data from the server
            // Note: clientAuth is used here as a convenient wrapper for sending authenticated commands.
            clientAuth.SendAuthenticatedCommand($"C_REQUEST_INITIAL_WORLD_STATE|{playerId}");

            // 3. Load the main game scene
            //LoadMainGameScene();

            // 4. Update UI (e.g., hide the main menu/loading screen)
            // UIManager.Instance.HideConnectingUI(); 

            Debug.Log("[GameFlow] Initial requests sent and scene load initiated.");
        }

        private void LoadMainGameScene()
        {
            if (!string.IsNullOrEmpty(mainGameSceneName))
            {
                Debug.Log($"[GameFlow] Loading scene: {mainGameSceneName}...");
                // Use LoadSceneMode.Single to unload the current scene and load the game scene
                SceneManager.LoadScene(mainGameSceneName, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogWarning("[GameFlow] Main game scene name is not set. Assuming current scene is the game scene.");
            }
        }
    }
}