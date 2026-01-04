namespace CosmicScavengers.Networking.Channel.Data
{
    public readonly struct ChannelData
    {
        public readonly byte[] RawBytes { get; }
        public readonly string[] TextParts { get; }

        public ChannelData(byte[] bytes)
        {
            RawBytes = bytes;
            TextParts = null;
        }

        public ChannelData(string[] text)
        {
            TextParts = text;
            RawBytes = null;
        }
    }
}
