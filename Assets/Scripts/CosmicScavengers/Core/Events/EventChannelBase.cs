using UnityEngine;
using UnityEngine.Events;

namespace CosmicScavengers.Core.Events
{
    /// <summary>
    /// A non-generic base for EventChannel to allow for lists of different types of event channels.
    /// </summary>
    public abstract class EventChannelBase : ScriptableObject
    {
        public abstract void AddListener(UnityAction<object> listener);
        public abstract void RemoveListener(UnityAction<object> listener);
    }
}
