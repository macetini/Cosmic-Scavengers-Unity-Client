using System;
using CosmicScavengers.Networking.Commands.Text;
using CosmicScavengers.Networking.Responses.Data.Text;

namespace CosmicScavengers.Networking.Responses.Registry.Meta
{
    [Serializable]
    public struct TextResponseEntry
    {
        public NetworkTextCommand Command;
        public BaseTextResponse Prefab;
    }
}
