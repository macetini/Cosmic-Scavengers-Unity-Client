using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Channels
{
    [CreateAssetMenu(menuName = "Channels/Responses/BinaryResponseChannel")]
    public class BinaryResponseChannel : EventChannel<NetworkBinaryCommand, byte[]> { }
}
