using System.Collections.Generic;
using CosmicScavengers.Networking.Event.Channels;
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
        private UnityEvent<List<object>> onEventRaised;

        private void OnEnable() => eventChannel.AddListener(onEventRaised.Invoke);

        private void OnDisable() => eventChannel.RemoveListener(onEventRaised.Invoke);
    }
}