using CosmicScavengers.Core.Events;
using CosmicScavengers.Core.Networking.Commands.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Channel.Response
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingResponseChannel")]
    public class NetworkingResponseChannel : EventChannel<BaseNetworkCommand, ResponseData> { }
}
