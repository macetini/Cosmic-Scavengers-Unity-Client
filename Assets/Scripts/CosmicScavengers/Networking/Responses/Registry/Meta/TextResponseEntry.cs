using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Responses.Data;

namespace CosmicScavengers.Networking.Responses.Registry.Meta
{
    [Serializable]
    public struct TextResponseEntry
    {
        public NetworkTextCommand Command;
        public BaseTextResponse Prefab;
    }
}
