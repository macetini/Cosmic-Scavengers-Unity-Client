namespace CosmicScavengers.Core.Networking.Commands.Channel.Outbound
{
    public readonly struct OutboundData
    {
        public readonly byte[] RawBytes { get; }
        public readonly int DataLength { get; }
        public static OutboundData Empty = new(new byte[0], 0);

        public OutboundData(byte[] bytes, int length)
        {
            RawBytes = bytes;
            DataLength = length;
        }
    }
}
