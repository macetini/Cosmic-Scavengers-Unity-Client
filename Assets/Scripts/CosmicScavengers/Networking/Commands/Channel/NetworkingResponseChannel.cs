using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Channel.Data;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingResponseChannel")]
    public class NetworkingResponseChannel : EventChannel<BaseNetworkCommand, ResponseData> { }
}
