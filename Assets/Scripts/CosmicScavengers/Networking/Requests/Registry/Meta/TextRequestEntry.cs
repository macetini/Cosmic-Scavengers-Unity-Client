using System;
using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Text.Data;

namespace CosmicScavengers.Networking.Requests.Registry.Meta
{
    [Serializable]
    public struct TextRequestEntry
    {
        public NetworkTextCommand Command;
        public BaseTextRequest Prefab;
    }
}
