using System.IO;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Extensions;
using CosmicScavengers.Networking.Event.Channels;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests
{
    public class WorldStateRequest
    {
        private readonly MemoryStream memoryStream;
        private readonly BinaryWriter binaryWriter;
        private readonly BinaryCommandChannel channel;
        private short CommandCode => (short)NetworkBinaryCommand.REQUEST_WORLD_STATE_C;

        public WorldStateRequest(BinaryCommandChannel channel)
        {
            memoryStream = new();
            binaryWriter = new(memoryStream);

            this.channel = channel;
        }

        public void Request(long userId)
        {
            binaryWriter.WriteShort(CommandCode);
            binaryWriter.WriteLong(userId);

            byte[] requestData = memoryStream.ToArray();

            Debug.Log("[WorldStateRequest] Sending world state request for User ID: " + userId);
            channel.Raise(requestData);
        }
    }
}
