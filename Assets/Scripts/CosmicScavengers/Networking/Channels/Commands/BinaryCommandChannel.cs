using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels.Commands
{
    [CreateAssetMenu(menuName = "Channels/Commands/BinaryCommandChannel")]
    public class BinaryCommandChannel : EventChannel<byte[]> { }
}
