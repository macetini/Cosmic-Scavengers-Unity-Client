namespace CosmicScavengers.Core.Networking.Request.Data.Meta
{
    public interface INetworkResponse
    {
        void Execute(params object[] parameters);
        void OnDestroy();
    }
}
