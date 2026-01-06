namespace CosmicScavengers.Networking.Channel.Data.Response
{
    public readonly struct ResponseData
    {
        public readonly byte[] RawBytes { get; }
        public readonly string[] TextParts { get; }
        public readonly int DataLength { get; }

        public static ResponseData Empty = new(new byte[0], 0);

        public ResponseData(byte[] bytes, int length)
        {
            RawBytes = bytes;
            DataLength = length;
            TextParts = null;
        }

        public ResponseData(string[] textParts, int length)
        {
            RawBytes = new byte[0];
            DataLength = textParts.Length;
            TextParts = textParts;
        }
    }
}
