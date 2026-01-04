using System;
using CosmicScavengers.Networking.Request.Binary.Data;
using CosmicScavengers.Networking.Commands;

namespace CosmicScavengers.Networking.Requests.Registry.Meta
{
    [Serializable]
    public struct BinaryRequestEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryRequest Prefab;
    }
}
