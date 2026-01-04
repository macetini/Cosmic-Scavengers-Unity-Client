using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Requests.Channel;
using UnityEngine;

namespace CosmicScavengers.Networking.Channel
{
    [CreateAssetMenu(menuName = "Channels/Networking/RequestChannel")]
    public class RequestChannel : EventChannel<BaseNetworkCommand, RequestChannelData> { }
}
