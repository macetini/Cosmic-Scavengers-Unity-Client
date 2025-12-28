using System;
using System.IO;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data.Meta;
using CosmicScavengers.Networking.Event.Channels.Commands;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseBinaryRequest : MonoBehaviour, INetworkRequest
    {
        public bool Active = true;

        [Header("Channel configuration")]
        [SerializeField]
        protected BinaryCommandChannel CommandChannel;

        public virtual NetworkBinaryCommand Command
        {
            get => throw new NotImplementedException();
        }

        protected MemoryStream Stream;
        protected BinaryWriter Writer;

        protected virtual void Awake()
        {
            Stream = new MemoryStream(1024 * 1024);
            Writer = new BinaryWriter(Stream);
        }

        protected virtual void Start()
        {
            if (CommandChannel == null)
            {
                Debug.LogError(
                    $"[BaseBinaryRequest] CommandChannel is missing on {gameObject.name}!"
                );
            }
        }

        public virtual void Execute(params object[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finalizes the binary buffer and raises the event on the Command Channel.
        /// </summary>
        protected void SendBuffer()
        {
            CommandChannel.Raise(Stream.ToArray());

            Stream.SetLength(0);
            Stream.Position = 0;
        }

        public void OnDestroy()
        {
            Writer?.Dispose();
            Stream?.Dispose();
        }
    }
}
