using CosmicScavengers.Core.Events.Binary;
using CosmicScavengers.Core.Networking.Commands.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Channel.Outbound
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingOutboundChannel")]
    public class NetworkingOutboundChannel : EventChannel<BaseNetworkCommand, OutboundData> { }
}
