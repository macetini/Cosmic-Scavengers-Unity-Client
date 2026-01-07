using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Core.Events
{
    /// <summary>
    /// A generic, reusable event channel that uses a ScriptableObject as the event bus.
    /// This allows for highly decoupled communication between different parts of the game.
    /// T is the type of the payload to be sent with the event.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the data to be passed with the event.</typeparam>
    public abstract class EventChannel<T> : EventChannelBase
    {
        [Tooltip("The action to perform when this event is raised.")]
        private UnityAction<T> onEventRaised;

        // A dictionary to keep track of the wrapped listeners for correct removal.
        private readonly Dictionary<UnityAction<object>, UnityAction<T>> wrappedListeners = new();

        public void Raise(T value)
        {
            onEventRaised?.Invoke(value);
        }

        public void AddListener(UnityAction<T> listener) => onEventRaised += listener;

        public override void AddListener(UnityAction<object> listener)
        {
            if (!wrappedListeners.ContainsKey(listener))
            {
                void wrappedListener(T value) => listener.Invoke(value);
                wrappedListeners[listener] = wrappedListener;
                onEventRaised += wrappedListener;
            }
        }

        public void RemoveListener(UnityAction<T> listener) => onEventRaised -= listener;

        public override void RemoveListener(UnityAction<object> listener)
        {
            if (wrappedListeners.TryGetValue(listener, out UnityAction<T> wrappedListener))
            {
                onEventRaised -= wrappedListener;
                wrappedListeners.Remove(listener);
            }
        }
    }
}
