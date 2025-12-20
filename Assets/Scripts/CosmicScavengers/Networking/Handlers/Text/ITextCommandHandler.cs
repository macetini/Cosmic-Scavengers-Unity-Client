namespace CosmicScavengers.Networking.Handlers.Text
{
    public interface ITextCommandHandler
    {
        NetworkTextCommand Command { get; }
        void Handle(string[] data);
    }
}
