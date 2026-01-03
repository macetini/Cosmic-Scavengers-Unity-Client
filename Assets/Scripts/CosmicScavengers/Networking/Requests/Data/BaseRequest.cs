using System;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseRequest<T> : MonoBehaviour
    {
        public bool Active = true;
        public T Data { get; set; }

        public void Execute(T[] parameters)
        {
            throw new NotImplementedException();
        }

        protected abstract void Raise();
    }
}
