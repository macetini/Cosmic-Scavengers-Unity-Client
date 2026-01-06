using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Channel.Data.Response;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel.Response
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingResponseChannel")]
    public class NetworkingResponseChannel : EventChannel<BaseNetworkCommand, ResponseData> { }
}
