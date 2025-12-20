using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    [CreateAssetMenu(menuName = "Channels/BinaryCommandChannel")]
    public class BinaryCommandChannel : EventChannel<byte[]> { }
}
