namespace CosmicScavengers.Networking.Handlers
{
    public interface INetworkRequestHandler
    {
        NetworkCommand CommandCode { get; }
        void Handle(byte[] protobufData);
    }
}
