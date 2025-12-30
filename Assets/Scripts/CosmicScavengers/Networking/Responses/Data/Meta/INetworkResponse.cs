namespace CosmicScavengers.Core.Networking.Request.Data.Meta
{
    public interface INetworkResponse
    {
        void Handle(byte[] parameters);
        void OnDestroy();
    }
}
