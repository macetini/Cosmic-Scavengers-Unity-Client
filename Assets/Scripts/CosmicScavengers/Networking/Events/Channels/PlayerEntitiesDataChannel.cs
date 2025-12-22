using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    [CreateAssetMenu(menuName = "Channels/PlayerEntitiesDataChannel")]
    public class PlayerEntitiesDataChannel : EventChannel<PlayerEntityListData> { }
}
