using System;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Networking.Commands.Responses.Data.Binary;

namespace CosmicScavengers.Networking.Commands.Responses.Registry.Meta
{
    [Serializable]
    public struct BinaryResponseEntry
    {
        public NetworkBinaryCommand Command;
        public BaseBinaryResponse Prefab;
    }
}
