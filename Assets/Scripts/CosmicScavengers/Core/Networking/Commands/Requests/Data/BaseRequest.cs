using System.IO;
using CosmicScavengers.Core.Networking.Commands.Channel.Outbound;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Requests.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseRequest<T> : MonoBehaviour, IRequest<T>
    {
        [Header("Status")]
        [Tooltip("Whether this request is currently active and should be processed.")]
        public bool Active = true;

        [Header("Channel Configuration")]
        [Tooltip("Channel for outgoing networking messages.")]
        [SerializeField]
        protected NetworkingOutboundChannel outboundChannel;

        protected MemoryStream Stream;

        protected virtual void Awake()
        {
            if (outboundChannel == null) // TODO - Find a way to show this error in the derived class - show a class name.
            {
                Debug.LogError($"NetworkingOutboundChannel is not assigned in BaseRequest.");
            }
            Stream = new MemoryStream(4096); // TODO - Capacity should ideally come from a config
        }

        /// <summary>
        /// Public entry point to trigger the request.
        /// </summary>
        public abstract void Execute(T[] parameters);

        /// <summary>
        /// Logic for writing specific parameters into the Stream.
        /// Returns true if packing was successful.
        /// </summary>
        protected abstract bool PackParameters(T[] parameters);

        /// <summary>
        /// Finalizes the internal buffer and broadcasts it via the NetworkingChannel.
        /// </summary>
        protected abstract void Raise();

        /// <summary>
        /// Centralized cleanup.
        /// </summary>
        protected virtual void OnDestroy()
        {
            Stream?.Dispose();
        }
    }
}
