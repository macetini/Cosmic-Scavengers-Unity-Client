using System;
using System.IO;
using CosmicScavengers.Networking.Extensions;

namespace CosmicScavengers.Networking.Requests
{
    public class WorldStateRequest
    {
        public static byte[] GetData(long userId)
        {
            MemoryStream memoryStream = new();
            BinaryWriter binaryWriter = new(memoryStream);

            binaryWriter.WriteShort((short)NetworkBinaryCommand.REQUEST_WORLD_STATE_C);
            binaryWriter.WriteLong(userId);

            return memoryStream.ToArray();
        }
    }
}
