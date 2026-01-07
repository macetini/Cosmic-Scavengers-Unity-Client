using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Event.Channels.Data
{
    [CreateAssetMenu(menuName = "Channels/Data/WorldDataChannel")]
    public class WorldDataChannel : EventChannel<WorldData> { }
}
