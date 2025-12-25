using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.Entities;
using Google.Protobuf.Collections;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    [CreateAssetMenu(menuName = "Channels/PlayerEntitiesDataChannel")]
    public class PlayerEntitiesDataChannel : EventChannel<EntitySyncResponse> { }
}
