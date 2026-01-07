using CosmicScavengers.Core.Events;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Event.Channels.Data
{
    [CreateAssetMenu(menuName = "Channels/Sync/EntitySyncChannel")]
    public class EntitySyncChannel : EventChannel<object> { }
}
