using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Requests.Channels
{
    [CreateAssetMenu(menuName = "Channels/Requests/RequestCommandChannel")]
    public class RequestCommandChannel : EventChannel<NetworkCommand, object> { }
}
