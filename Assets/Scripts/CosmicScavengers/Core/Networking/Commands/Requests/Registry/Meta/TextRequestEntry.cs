using System;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Request.Data.Text;

namespace CosmicScavengers.Core.Networking.Commands.Request.Registry.Meta
{
    [Serializable]
    public struct TextRequestEntry
    {
        public NetworkTextCommand Command;
        public BaseTextRequest Prefab;
    }
}
