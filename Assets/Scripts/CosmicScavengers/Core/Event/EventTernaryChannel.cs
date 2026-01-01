using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Core.Event
{
    public abstract class EventChannel<T1, T2, T3> : EventChannelBase
    {
        [Tooltip("The action to perform when this event is raised.")]
        private UnityAction<T1, T2, T3> onEventRaised;

        // A dictionary to keep track of the wrapped listeners for correct removal.
        private readonly Dictionary<UnityAction<object>, UnityAction<T1, T2, T3>> wrappedListeners =
            new();

        public void Raise(T1 val1, T2 val2, T3 val3)
        {
            onEventRaised?.Invoke(val1, val2, val3);
        }

        public void AddListener(UnityAction<T1, T2, T3> listener) => onEventRaised += listener;

        public override void AddListener(UnityAction<object> listener)
        {
            if (!wrappedListeners.ContainsKey(listener))
            {
                // We wrap the two generic values into a single object (Tuple) to satisfy the base signature
                void wrappedListener(T1 v1, T2 v2, T3 v3) => listener.Invoke((v1, v2, v3));

                wrappedListeners[listener] = wrappedListener;
                onEventRaised += wrappedListener;
            }
        }

        public void RemoveListener(UnityAction<T1, T2, T3> listener) => onEventRaised -= listener;

        public override void RemoveListener(UnityAction<object> listener)
        {
            if (wrappedListeners.TryGetValue(listener, out UnityAction<T1, T2, T3> wrappedListener))
            {
                onEventRaised -= wrappedListener;
                wrappedListeners.Remove(listener);
            }
        }
    }
}
