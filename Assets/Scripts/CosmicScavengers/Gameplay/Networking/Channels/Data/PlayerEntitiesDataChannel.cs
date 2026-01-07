using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.Entities;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Event.Channels.Data
{
    [CreateAssetMenu(menuName = "Channels/Data/PlayerEntitiesDataChannel")]
    public class PlayerEntitiesDataChannel : EventChannel<EntitySyncResponse> { }
}
