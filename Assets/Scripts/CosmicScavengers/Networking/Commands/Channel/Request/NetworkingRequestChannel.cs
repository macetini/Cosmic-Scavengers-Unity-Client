using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Channel.Data.Request;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel.Request
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingRequestChannel")]
    public class NetworkingRequestChannel : EventChannel<BaseNetworkCommand, RequestData> { }
}
