using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Core.Events
{
    /// <summary>
    /// A generic, reusable event channel that uses a ScriptableObject as the event bus.
    /// This allows for highly decoupled communication between different parts of the game.
    /// T is the type of the payload to be sent with the event.
    /// </summary>
    /// <typeparam name="T">The type of the data to be passed with the event.</typeparam>
    public abstract class EventChannel<T> : ScriptableObject
    {
        [Tooltip("The action to perform when this event is raised.")]
        private UnityAction<T> onEventRaised;

        public void Raise(T value)
        {
            onEventRaised?.Invoke(value);
        }

        public void AddListener(UnityAction<T> listener) => onEventRaised += listener;
        
        public void RemoveListener(UnityAction<T> listener) => onEventRaised -= listener;
    }
}