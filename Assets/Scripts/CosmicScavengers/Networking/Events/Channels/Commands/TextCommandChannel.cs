using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels.Commands
{
    [CreateAssetMenu(menuName = "Channels/Data/TextCommandChannel")]
    public class TextCommandChannel : EventChannel<string> { }
}
