using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Binary.Data;

namespace CosmicScavengers.Networking.Requests.Registry.Meta
{
    [Serializable]
    public struct BinaryRequestEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryRequest Prefab;
    }
}
