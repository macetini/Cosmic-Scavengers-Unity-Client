using UnityEngine;

namespace CosmicScavengers.Core
{
    /// <summary>
    /// The master controller for game state. It waits for successful authentication 
    /// and then triggers the transition to the main game scene or world loading.
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
    
        private bool isGameFlowStarted = false;

        /// <summary>
        /// Triggered directly by the ClientAuth component after a successful S_LOGIN_OK.
        /// This is the entry point into the actual game.
        /// </summary>
        /// <param name="playerId">The ID of the successfully authenticated player.</param>
        public void StartGameSession(long playerId)
        {
            if (isGameFlowStarted)
            {
                Debug.LogWarning("[GameFlowController] StartGameSession called multiple times. Ignoring subsequent calls.");
                return;
            }

            isGameFlowStarted = true;
            Debug.Log($"[GameFlow] Login confirmed with ID: {playerId}. Beginning asset loading and scene transition.");

            // 2. Request initial world data from the server
            
            // 3. Load the main game scene (uncomment when ready)
            //LoadMainGameScene();

            // 4. Update UI (e.g., hide the main menu/loading screen)
            // UIManager.Instance.HideConnectingUI(); 
            Debug.Log("[GameFlow] Initial requests sent and scene load initiated.");
        }
    }
}