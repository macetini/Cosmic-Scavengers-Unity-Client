namespace CosmicScavengers.Core.Networking.Commands.Channel.Inbound
{
    public readonly struct InboundData
    {
        public readonly byte[] RawBytes { get; }
        public readonly string[] TextParts { get; }
        public readonly int DataLength { get; }

        public static InboundData Empty = new(new byte[0], 0);

        public InboundData(byte[] bytes, int length)
        {
            RawBytes = bytes;
            DataLength = length;
            TextParts = null;
        }

        public InboundData(string[] textParts, int length)
        {
            RawBytes = new byte[0];
            DataLength = textParts.Length;
            TextParts = textParts;
        }
    }
}
