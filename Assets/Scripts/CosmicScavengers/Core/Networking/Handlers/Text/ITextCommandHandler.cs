using CosmicScavengers.Core.Networking.Commands;

namespace CosmicScavengers.Core.Networking.Handlers.Text
{
    public interface ITextCommandHandler
    {
        NetworkTextCommand Command { get; }
        void Handle(string[] data);
    }
}
