using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Event.Channels.Data
{
    [CreateAssetMenu(menuName = "Channels/Sync/EntitySyncChannel")]
    public class EntitySyncChannel : EventChannel<object> { }
}
