namespace CosmicScavengers.Networking.Channel.Data
{
    public readonly struct RequestData
    {
        public readonly byte[] RawBytes { get; }
        public readonly int DataLength { get; }
        public static ResponseData Empty = new(new byte[0], 0);

        public RequestData(byte[] bytes, int length)
        {
            RawBytes = bytes;
            DataLength = length;
        }
    }
}
