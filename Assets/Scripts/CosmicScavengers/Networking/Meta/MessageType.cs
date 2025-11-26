using UnityEngine;

namespace Assets.Scripts.CosmicScavengers.Networking.Meta
{
    public enum MessageType : byte
    {
        TEXT = 0x01, // Used for Auth, Chat, Lobby Commands
        BINARY = 0x02 // Used for Game State, Unit Positions, Physics
    }
}
