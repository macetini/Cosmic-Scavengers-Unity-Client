using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Connector.Dispatcher.Channel
{
    [CreateAssetMenu(menuName = "Network/MessageDispatchChannel")]
    public class MessageDispatchChannel : EventChannel<CommandType, object> { }
}
