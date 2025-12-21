using CosmicScavengers.Core.Networking.Commands;

namespace CosmicScavengers.Core.Networking.Handlers.Binary
{
    public interface IBinaryCommandHandler
    {
        NetworkBinaryCommand Command { get; }
        void Handle(byte[] protobufData);
    }
}
