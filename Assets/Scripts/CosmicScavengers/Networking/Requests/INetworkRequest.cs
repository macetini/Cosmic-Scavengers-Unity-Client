namespace CosmicScavengers.Networking.Requests
{
    public interface INetworkRequest
    {
        NetworkCommand CommandCode { get; }
        void Dispatch(ClientConnector clientConnector, params object[] parameters);
    }
}
