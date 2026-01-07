using System;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Commands.Responses.Data.Text;

namespace CosmicScavengers.Networking.Commands.Responses.Registry.Meta
{
    [Serializable]
    public struct TextResponseEntry
    {
        public NetworkTextCommand Command;
        public BaseTextResponse Prefab;
    }
}
