using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Models;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    /// <summary>
    /// Event channel for broadcasting when world data is received.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/GetWorldDataEventChannel")]
    public class GetWorldDataEventChannel : EventChannel<WorldData> { }
}