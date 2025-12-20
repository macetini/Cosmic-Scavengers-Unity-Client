namespace CosmicScavengers.Networking.Requests
{
    public interface INetworkRequest
    {
        NetworkBinaryCommand CommandCode { get; }
        void Dispatch(ClientConnector clientConnector, params object[] parameters);
    }
}
