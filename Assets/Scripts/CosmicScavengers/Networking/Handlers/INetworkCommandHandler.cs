using System.Diagnostics.Tracing;

namespace CosmicScavengers.Networking.Handlers
{
    public interface INetworkCommandHandler
    {        
        short CommandCode { get; }
        void Handle(byte[] protobufData);
    }
}
