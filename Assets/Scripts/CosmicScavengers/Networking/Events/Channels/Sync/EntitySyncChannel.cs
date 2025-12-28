using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels.Data
{
    [CreateAssetMenu(menuName = "Channels/Sync/EntitySyncChannel")]
    public class EntitySyncChannel : EventChannel<object> { }
}
