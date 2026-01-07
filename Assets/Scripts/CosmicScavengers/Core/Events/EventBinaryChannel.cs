using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Core.Events
{
    public abstract class EventChannel<T1, T2> : EventChannelBase
    {
        [Tooltip("The action to perform when this event is raised.")]
        private UnityAction<T1, T2> onEventRaised;

        // Since the base class expects a single 'object' for type-erased listeners,
        // we wrap the two arguments into an ValueTuple or Array when calling base listeners.
        private readonly Dictionary<UnityAction<object>, UnityAction<T1, T2>> wrappedListeners =
            new();

        public void Raise(T1 val1, T2 val2)
        {
            onEventRaised?.Invoke(val1, val2);
        }

        public void AddListener(UnityAction<T1, T2> listener) => onEventRaised += listener;

        /// <summary>
        /// Implementation of the base typeless listener.
        /// Note: The base listener will receive a ValueTuple containing both values.
        /// </summary>
        public override void AddListener(UnityAction<object> listener)
        {
            if (!wrappedListeners.ContainsKey(listener))
            {
                // We wrap the two generic values into a single object (Tuple) to satisfy the base signature
                void wrappedListener(T1 v1, T2 v2) => listener.Invoke((v1, v2));

                wrappedListeners[listener] = wrappedListener;
                onEventRaised += wrappedListener;
            }
        }

        public void RemoveListener(UnityAction<T1, T2> listener) => onEventRaised -= listener;

        public override void RemoveListener(UnityAction<object> listener)
        {
            if (wrappedListeners.TryGetValue(listener, out UnityAction<T1, T2> wrappedListener))
            {
                onEventRaised -= wrappedListener;
                wrappedListeners.Remove(listener);
            }
        }
    }
}
