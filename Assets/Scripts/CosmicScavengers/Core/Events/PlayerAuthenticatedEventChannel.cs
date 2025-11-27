using UnityEngine;

namespace CosmicScavengers.Core.Events
{
    /// <summary>
    /// An event channel specifically for broadcasting when a player has been successfully authenticated.
    /// The payload is the player's ID.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/PlayerAuthenticatedEventChannel")]
    public class PlayerAuthenticatedEventChannel : EventChannel<long> { }
}