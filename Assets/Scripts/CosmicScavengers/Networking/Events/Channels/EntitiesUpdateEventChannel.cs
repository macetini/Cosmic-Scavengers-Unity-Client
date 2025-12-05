using System.Collections.Generic;
using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    /// <summary>
    /// Event channel for broadcasting updates to entity data.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/EntitiesUpdateEventChannel")]
    public class EntitiesUpdateEventChannel : EventChannel<List<EntityData>> { }
}