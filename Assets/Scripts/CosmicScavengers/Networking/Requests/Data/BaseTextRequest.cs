using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Networking.Event.Channels.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseTextRequest : MonoBehaviour
    {
        public bool Active = true;

        [Header("Channel configuration")]
        [SerializeField]
        [Tooltip("The command channel to raise the text command on.")]
        protected TextCommandChannel CommandChannel;

        public virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }

        protected virtual void Awake()
        {
            if (CommandChannel == null)
            {
                Debug.LogError("[BaseBinaryRequest] CommandChannel reference is missing!");
            }
        }

        public virtual void Execute(object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
