namespace CosmicScavengers.Networking.Handlers.Binary
{
    public interface IBinaryCommandHandler
    {
        NetworkBinaryCommand Command { get; }
        void Handle(byte[] protobufData);
    }
}
