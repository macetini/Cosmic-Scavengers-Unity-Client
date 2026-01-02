namespace CosmicScavengers.Core.Networking.Request.Data.Meta
{
    public interface INetworkRequest<T>
    {
        T Data { set; }
        void Execute(T[] parameters);
    }
}
