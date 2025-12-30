using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Responses.Data;

namespace CosmicScavengers.Networking.Responses.Registry.Meta
{
    [Serializable]
    public struct BinaryResponseEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryResponse Prefab;
    }
}
