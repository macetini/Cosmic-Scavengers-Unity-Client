using System;
using CosmicScavengers.Networking.Channel;
using UnityEngine;

namespace CosmicScavengers.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseRequest<T> : MonoBehaviour
    {
        [Header("Active")]
        public bool Active = true;

        [Header("RequestChannel")]
        [SerializeField]
        protected NetworkingChannel networkingChannel;

        void Awake()
        {
            if (networkingChannel == null)
            {
                Debug.LogError(
                    $"[{gameObject.name}]: RequestChannel is not assigned in BaseRequest."
                );
            }
        }

        public abstract void Execute(T[] parameters);
        protected abstract void Raise();
    }
}
