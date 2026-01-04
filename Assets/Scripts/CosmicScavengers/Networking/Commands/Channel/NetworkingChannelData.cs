namespace CosmicScavengers.Networking.Channel.Data
{
    public readonly struct NetworkingChannelData
    {
        public readonly byte[] RawBytes { get; }
        public readonly string[] TextParts { get; }

        public NetworkingChannelData(byte[] bytes)
        {
            RawBytes = bytes;
            TextParts = null;
        }

        public NetworkingChannelData(string[] text)
        {
            TextParts = text;
            RawBytes = null;
        }
    }
}
