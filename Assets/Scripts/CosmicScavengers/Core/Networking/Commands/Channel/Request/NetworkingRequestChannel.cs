using CosmicScavengers.Core.Events;
using CosmicScavengers.Core.Networking.Commands.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Channel.Request
{
    [CreateAssetMenu(menuName = "Channels/Networking/NetworkingRequestChannel")]
    public class NetworkingRequestChannel : EventChannel<BaseNetworkCommand, RequestData> { }
}
