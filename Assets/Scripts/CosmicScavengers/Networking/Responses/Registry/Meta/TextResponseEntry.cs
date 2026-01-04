using System;
using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Responses.Data;

namespace CosmicScavengers.Networking.Responses.Registry.Meta
{
    [Serializable]
    public struct TextResponseEntry
    {
        public NetworkTextCommand Command;
        public BaseTextResponse Prefab;
    }
}
