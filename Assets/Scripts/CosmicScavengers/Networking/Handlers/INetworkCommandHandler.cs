namespace CosmicScavengers.Networking.Handlers
{
    public interface INetworkCommandHandler
    {
        NetworkCommand CommandCode { get; }
        void Handle(byte[] protobufData);
    }
}
