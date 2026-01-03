namespace CosmicScavengers.Core.Networking.Responses.Channels
{
    public readonly struct ResponseData
    {
        public readonly byte[] RawBytes;
        public readonly string[] TextParts;

        public ResponseData(byte[] bytes)
        {
            RawBytes = bytes;
            TextParts = null;
        }

        public ResponseData(string[] text)
        {
            TextParts = text;
            RawBytes = null;
        }
    }
}
