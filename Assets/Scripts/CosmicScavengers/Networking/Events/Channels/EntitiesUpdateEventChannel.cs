using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    /// <summary>
    /// Event channel for broadcasting updates to entity data.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/EntitiesUpdateEventChannel")]
    public class EntitiesUpdateEventChannel : EventChannel<PlayerEntityData> { }
}