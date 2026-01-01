using System.Data;
using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Requests.Channels
{
    [CreateAssetMenu(menuName = "Channels/Requests/GenericRequestChannel")]
    public class GenericRequestChannel : EventChannel<NetworkTextCommand, CommandType, object> { }
}
