using System;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Request.Data.Binary;

namespace CosmicScavengers.Core.Networking.Commands.Request.Registry.Meta
{
    [Serializable]
    public struct BinaryRequestEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryRequest Prefab;
    }
}
