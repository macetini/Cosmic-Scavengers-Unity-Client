using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Networking.Event.Listeners
{
    /// <summary>
    /// Listens for entity update events and invokes responses.
    /// </summary>
    public class EntitiesUpdateEventListener : MonoBehaviour
    {
        [Tooltip("The event channel to listen to.")]
        [SerializeField]
        private EntitiesUpdateEventChannel eventChannel;

        [Tooltip("The response to invoke when the event is raised.")]
        [SerializeField]
        private UnityEvent<PlayerEntityData> onEventRaised;

        private void OnEnable() => eventChannel.AddListener(onEventRaised.Invoke);

        private void OnDisable() => eventChannel.RemoveListener(onEventRaised.Invoke);
    }
}
