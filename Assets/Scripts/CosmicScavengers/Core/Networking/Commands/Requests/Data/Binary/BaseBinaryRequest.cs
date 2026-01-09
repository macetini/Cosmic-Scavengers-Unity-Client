using System.IO;
using System.Net;
using CosmicScavengers.Core.Networking.Commands.Channel.Outbound;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Commands.Requests.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Data.Binary
{
    /// <summary>
    /// Specialist for binary-serialized network requests.
    /// Manages the internal buffer and automatically handles Command ID serialization.
    /// </summary>
    public abstract class BaseBinaryRequest : BaseRequest<object>
    {
        protected virtual NetworkBinaryCommand Command { get; }
        protected BinaryWriter Writer;
        protected const int HEADER_SIZE = sizeof(short);

        protected override void Awake()
        {
            base.Awake();
            Stream = new MemoryStream(4096); // TODO - Put this in config
            Writer = new BinaryWriter(Stream);

            short id = (short)Command;
            Writer.Write(IPAddress.HostToNetworkOrder(id));
        }

        /// <summary>
        /// Default implementation for Text requests.
        /// Returns true by default so parameterless requests don't need to override it.
        /// </summary>
        protected override bool PackParameters(object[] parameters)
        {
            return true;
        }

        /// <summary>
        /// The Template Method. It handles the shared boilerplate of resetting the stream
        /// and writing the Command ID before calling the child-specific logic.
        /// </summary>
        public sealed override void Execute(object[] parameters)
        {
            if (!Active)
            {
                Debug.Log($"[{gameObject.name}]: Request for '{Command}' is not active.");
                return;
            }

            Stream.Position = HEADER_SIZE;

            bool packed = PackParameters(parameters);
            if (packed)
            {
                Stream.SetLength(Stream.Position);
                Raise();
            }
            else
            {
                Debug.LogWarning(
                    $"[{gameObject.name}]: Failed to pack parameters for '{Command}'. Request aborted."
                );
            }
        }

        /// <summary>
        /// Finalizes the binary buffer and raises the event on the Networking Channel.
        /// </summary>
        protected override void Raise()
        {
            byte[] internalBuffer = Stream.GetBuffer();
            int packetLength = (int)Stream.Length;
            OutboundData channelData = new(internalBuffer, packetLength);
            outboundChannel.Raise(Command, channelData);
        }

        protected override void OnDestroy()
        {
            Writer?.Dispose();
            base.OnDestroy();
        }
    }
}
