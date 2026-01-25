using System.ComponentModel;

namespace CosmicScavengers.Core.Networking.Commands.Data.Meta
{
    /// <summary>
    /// Defines the types of messages exchanged between client and server.
    /// </summary>
    public enum CommandType : byte
    {
        [Description("Unknown command.")]
        UNKNOWN = 0x00, // Used for unknown commands

        [Description("Text command.")]
        TEXT = 0x01, // Used for Auth, Chat, Lobby Commands

        [Description("Binary command.")]
        BINARY = 0x02, // Used for Game State, Unit Positions, Physics
    }
}
