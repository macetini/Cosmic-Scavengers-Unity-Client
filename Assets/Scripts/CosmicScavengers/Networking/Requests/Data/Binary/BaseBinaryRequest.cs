using System.IO;
using CosmicScavengers.Networking.Channel.Data;
using CosmicScavengers.Networking.Commands.Data.Binary;
using CosmicScavengers.Networking.Request.Data;

namespace CosmicScavengers.Networking.Request.Binary.Data
{
    /// <summary>
    /// Abstract base for binary-serialized network requests.
    /// Inherits from MonoBehaviour to support Unity-native discovery and Inspector configuration.
    /// </summary>
    public abstract class BaseBinaryRequest : BaseRequest<object[]>
    {
        protected virtual NetworkBinaryCommand Command { get; }

        protected MemoryStream Stream;
        protected BinaryWriter Writer;

        protected virtual void Awake()
        {
            Stream = new MemoryStream(1024 * 1024); // TODO - Put this in config
            Writer = new BinaryWriter(Stream);
        }

        /// <summary>
        /// Finalizes the binary buffer and raises the event on the Command Channel.
        /// </summary>
        protected override void Raise()
        {
            byte[] bytes = Stream.ToArray();

            ChannelData data = new(bytes);

            requestChannel.Raise(Command, data);

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
