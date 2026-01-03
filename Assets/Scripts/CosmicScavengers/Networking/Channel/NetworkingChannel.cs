using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Networking.Channel.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingChannel")]
    public class NetworkingChannel : EventChannel<NetworkCommand, ChannelData> { }
}
