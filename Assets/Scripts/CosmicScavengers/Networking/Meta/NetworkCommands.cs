namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Defines the command types for network messages.
    /// </summary>
    public static class NetworkCommands
    {
        public const short REQUEST_WORLD_DATA_C = 0x0001;
        public const short REQUEST_WORLD_DATA_S = 0x0002;
        public const short REQUEST_PLAYER_ENTITIES_C = 0x0003;
        public const short REQUEST_PLAYER_ENTITIES_S = 0x0004;
    }
}