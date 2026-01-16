using System.ComponentModel;

namespace CosmicScavengers.Core.Networking.Commands.Data.Binary
{
    /// <summary>
    /// Defines the command types for network messages.
    /// </summary>
    public enum NetworkBinaryCommand : short
    {
        [Description("Unknown command.")]
        UNKNOWN = 0x0000,

        [Description("Request the initial state of the game world.")]
        REQUEST_PLAYER_INIT_STATE_C = 0x0001,

        [Description("Send the current state of the game world.")]
        REQUEST_WORLD_STATE_S = 0x0002,

        [Description("Request the entities associated with a player.")]
        REQUEST_PLAYER_ENTITIES_C = 0x0003,

        [Description("Send the entities associated with a player.")]
        REQUEST_PLAYER_ENTITIES_S = 0x0004,

        [Description("Request an entity to movement.")]
        REQUEST_ENTITY_MOVE_C = 0x0005,

        [Description("Entity movement Server response.")]
        REQUEST_ENTITY_MOVE_S = 0x0006,
    }
}
