using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    [CreateAssetMenu(menuName = "Channels/WorldDataChannel")]
    public class WorldDataChannel : EventChannel<WorldData> { }
}
