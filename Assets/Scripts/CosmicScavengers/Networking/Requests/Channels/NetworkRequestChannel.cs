using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Requests.Channels
{
    [CreateAssetMenu(menuName = "Channels/Requests/NetworkRequestChannel")]
    public class NetworkRequestChannel : EventChannel<NetworkCommand, object> { }
}
