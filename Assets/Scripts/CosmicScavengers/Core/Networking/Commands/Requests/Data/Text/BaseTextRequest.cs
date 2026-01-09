using System.IO;
using System.Text;
using CosmicScavengers.Core.Networking.Commands.Channel.Outbound;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Requests.Data;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Request.Data.Text
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseTextRequest : BaseRequest<object>
    {
        protected virtual NetworkTextCommand Command { get; }
        protected StreamWriter TextWriter;
        protected int headerByteLength;
        private const string COMMAND_SEPARATOR = "|";

        protected override void Awake()
        {
            base.Awake();

            // TextWriter using UTF8 without a BOM (Byte Order Mark)
            // This ensures compatibility with most server-side parsers.
            TextWriter = new StreamWriter(Stream, new UTF8Encoding(false));

            // Encode the command name and the separator once at startup.
            TextWriter.Write($"{Command}{COMMAND_SEPARATOR}");
            TextWriter.Flush();

            // Record the byte length of the header
            headerByteLength = (int)Stream.Position;
        }

        /// <summary>
        /// Writes multiple arguments sequentially.
        /// </summary>
        protected void WriteArgs(params string[] args)
        {
            if (args == null)
            {
                return;
            }
            foreach (var arg in args)
            {
                WriteArg(arg);
            }
        }

        /// <summary>
        /// Writes a single argument to the stream.
        /// Automatically inserts a separator if an argument has already been written.
        /// </summary>
        protected void WriteArg(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return;
            }
            TextWriter.Write(arg);
            TextWriter.Write(COMMAND_SEPARATOR);
        }

        /// <summary>
        /// Default override when the request has no arguments to pack.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override bool PackParameters(object[] parameters)
        {
            return true;
        }

        /// <summary>
        /// The Template Method logic for Text requests.
        /// </summary>
        public sealed override void Execute(object[] parameters)
        {
            if (!Active)
            {
                Debug.Log($"[{gameObject.name}]: Request for '{Command}' is not active.");
                return;
            }

            // Seek to the start of the "data" section (immediately after the Command header)
            Stream.Position = headerByteLength;

            // Pack the parameters
            bool packed = PackParameters(parameters);
            if (packed)
            {
                // Flush the StreamWriter to push character bytes into the MemoryStream
                TextWriter.Flush();
                // Truncate to current position to remove leftover data from previous calls
                Stream.SetLength(Stream.Position);

                Raise();
            }
        }

        /// <summary>
        /// Finalizes the buffer and raises the event on the channel using zero-allocation bytes.
        /// </summary>
        protected override void Raise()
        {
            byte[] buffer = Stream.GetBuffer();
            int length = (int)Stream.Length;

            OutboundData data = new(buffer, length);
            outboundChannel.Raise(Command, data);
        }

        protected override void OnDestroy()
        {
            TextWriter?.Dispose();
            base.OnDestroy();
        }
    }
}
