using System;
using CosmicScavengers.Networking.Commands.Binary;
using CosmicScavengers.Networking.Request.Binary.Data;

namespace CosmicScavengers.Networking.Requests.Registry.Meta
{
    [Serializable]
    public struct BinaryRequestEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryRequest Prefab;
    }
}
