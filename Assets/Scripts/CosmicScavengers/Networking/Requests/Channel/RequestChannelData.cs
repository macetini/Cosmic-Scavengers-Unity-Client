namespace CosmicScavengers.Networking.Requests.Channel
{
    public readonly struct RequestChannelData
    {
        public readonly object[] ObjectParts { get; }
        public readonly string[] TextParts { get; }

        public static RequestChannelData Empty => new(null);

        public RequestChannelData(object[] bytes)
        {
            ObjectParts = bytes;
            TextParts = null;
        }

        public RequestChannelData(string[] text)
        {
            TextParts = text;
            ObjectParts = null;
        }
    }
}
