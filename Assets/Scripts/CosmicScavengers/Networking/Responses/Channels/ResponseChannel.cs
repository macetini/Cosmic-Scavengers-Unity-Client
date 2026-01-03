using CosmicScavengers.Core.Event;
using CosmicScavengers.Core.Networking.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Channels
{
    [CreateAssetMenu(menuName = "Channels/Responses/ResponseChannel")]
    public class ResponseChannel : EventChannel<NetworkCommand, ResponseData> { }
}
