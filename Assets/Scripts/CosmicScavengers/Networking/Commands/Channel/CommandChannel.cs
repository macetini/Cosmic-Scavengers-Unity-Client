using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Channel.Data;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingChannel")]
    public class CommandChannel : EventChannel<BaseNetworkCommand, ChannelData> { }
}
