using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Channels
{
    [CreateAssetMenu(menuName = "Channels/Requests/RequestCommandChannel")]
    public class RequestCommandChannel : EventChannel<BaseNetworkCommand, object> { }
}
