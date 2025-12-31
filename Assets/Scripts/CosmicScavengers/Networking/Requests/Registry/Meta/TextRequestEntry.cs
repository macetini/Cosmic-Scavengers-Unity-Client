using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data;

namespace CosmicScavengers.Networking.Requests.Registry.Meta
{
    [Serializable]
    public struct TextRequestEntry
    {
        public NetworkTextCommand Command;
        public BaseTextRequest Prefab;
    }
}
