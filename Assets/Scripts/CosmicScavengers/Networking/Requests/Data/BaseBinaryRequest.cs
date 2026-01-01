using System;
using System.IO;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data.Meta;
using CosmicScavengers.Core.Networking.Requests.Channels;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseBinaryRequest : BaseRequest<object[]>
    {
        [Header("Channel configuration")]
        [SerializeField]
        protected BinaryRequestChannel Channel;
        protected virtual NetworkBinaryCommand Command { get; }

        protected MemoryStream Stream;
        protected BinaryWriter Writer;

        protected virtual void Awake()
        {
            if (Channel == null)
            {
                Debug.LogError("[BaseBinaryRequest] CommandChannel reference is missing!");
            }
            Stream = new MemoryStream(1024 * 1024); // TODO - Put this in config
            Writer = new BinaryWriter(Stream);
        }

        public virtual void Execute(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finalizes the binary buffer and raises the event on the Command Channel.
        /// </summary>
        protected void SendBuffer()
        {
            Channel.Raise(Command, Stream.ToArray());

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
