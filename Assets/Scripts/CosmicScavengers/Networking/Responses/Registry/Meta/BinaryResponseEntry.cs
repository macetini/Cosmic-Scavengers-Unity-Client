using System;
using CosmicScavengers.Networking.Commands.Binary;
using CosmicScavengers.Networking.Responses.Data;

namespace CosmicScavengers.Networking.Responses.Registry.Meta
{
    [Serializable]
    public struct BinaryResponseEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryResponse Prefab;
    }
}
