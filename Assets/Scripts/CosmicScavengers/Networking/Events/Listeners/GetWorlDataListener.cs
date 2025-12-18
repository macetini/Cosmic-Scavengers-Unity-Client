using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Networking.Event.Listeners
{
    public class GetWorldDataListener : MonoBehaviour
    {
        [Tooltip("The event channel to listen to.")]
        [SerializeField]
        private GetWorldDataEventChannel eventChannel;

        [Tooltip("The response to invoke when the event is raised.")]
        [SerializeField]
        private UnityEvent<WorldData> onEventRaised;

        private void OnEnable() => eventChannel.AddListener(onEventRaised.Invoke);

        private void OnDisable() => eventChannel.RemoveListener(onEventRaised.Invoke);
    }
}
