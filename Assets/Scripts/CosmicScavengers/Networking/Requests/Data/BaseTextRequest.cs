using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Requests.Channels;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseTextRequest : BaseRequest<string>
    {
        [Header("Channel configuration")]
        [SerializeField]
        [Tooltip("The command channel to raise the text command on.")]
        protected TextRequestChannel Channel;

        protected virtual NetworkTextCommand Command
        {
            get => throw new NotImplementedException();
        }

        protected virtual void Awake()
        {
            if (Channel == null)
            {
                Debug.LogError("[BaseTextRequest] Channel reference is missing!");
            }
        }
    }
}
