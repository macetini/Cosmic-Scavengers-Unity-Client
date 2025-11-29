using UnityEngine;
using System.IO;

namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Listens for high-level game events and translates them into network requests.
    /// This acts as a bridge between the game logic and the low-level ClientConnector.
    /// </summary>
    public class NetworkRequestManager : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private ClientConnector clientConnector;

        /// <summary>
        /// Sends a request to the server to retrieve the initial world state for the authenticated player.
        /// 
        /// <param name="playerId">The ID of the authenticated player.</param>        
        /// 
        /// </summary>
        public void OnRequestWorldState(long playerId)
        {
            Debug.Log($"[NetworkRequestManager] Sending world state request for Player ID: {playerId}");
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.Write(NetworkCommands.REQUEST_WORLD_STATE);
            writer.Write(playerId);
            clientConnector.SendBinaryMessage(memoryStream.ToArray());
        }
    }
}