using CosmicScavengers.Networking.Event.Channels;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Networking.Event.Listeners
{
    /// <summary>
    /// A component that listens to a PlayerAuthenticatedEventChannel and invokes a UnityEvent in response.
    /// This allows wiring up game logic in the Inspector without writing custom code for each listener.
    /// </summary>
    public class PlayerAuthenticatedListener : MonoBehaviour
    {
        [Tooltip("The event channel to listen to.")]
        [SerializeField]
        private PlayerAuthenticatedEventChannel eventChannel;

        [Tooltip("The response to invoke when the event is raised.")]
        [SerializeField]
        private UnityEvent<long> onEventRaised;

        private void OnEnable() => eventChannel.AddListener(onEventRaised.Invoke);

        private void OnDisable() => eventChannel.RemoveListener(onEventRaised.Invoke);
    }
}