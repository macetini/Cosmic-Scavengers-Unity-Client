using UnityEngine;

namespace CosmicScavengers.Networking.Communication
{
    /// <summary>
    /// Central registry for all network command handlers in the scene.
    /// Use the Context Menu "Assign Handlers" to verify registrations in the Inspector.
    /// </summary>
    public class BinaryCommandHandlers : MonoBehaviour
    {
        public void Handle(short command, byte[] data) { }
    }
}
