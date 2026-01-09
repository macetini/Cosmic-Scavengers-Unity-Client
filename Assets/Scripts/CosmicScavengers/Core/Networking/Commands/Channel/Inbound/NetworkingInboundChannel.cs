using CosmicScavengers.Core.Events.Binary;
using CosmicScavengers.Core.Networking.Commands.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Channel.Inbound
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingInboundChannel")]
    public class NetworkingInboundChannel : EventChannel<BaseNetworkCommand, InboundData> { }
}
