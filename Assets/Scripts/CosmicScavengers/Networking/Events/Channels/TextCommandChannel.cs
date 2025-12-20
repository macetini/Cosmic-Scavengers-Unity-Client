using CosmicScavengers.Core.Event;
using UnityEngine;

namespace CosmicScavengers.Networking.Event.Channels
{
    [CreateAssetMenu(menuName = "Channels/TextCommandChannel")]
    public class TextCommandChannel : EventChannel<string> { }
}
