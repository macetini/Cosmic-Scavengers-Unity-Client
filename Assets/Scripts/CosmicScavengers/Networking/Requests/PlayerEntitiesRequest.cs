using System.IO;
using CosmicScavengers.Networking;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Extensions;
using UnityEngine;

public class PlayerEntitiesRequest
{
    private readonly MemoryStream memoryStream;
    private readonly BinaryWriter binaryWriter;
    private readonly BinaryCommandChannel channel;
    private short CommandCode => (short)NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_C;

    public PlayerEntitiesRequest(BinaryCommandChannel channel)
    {
        memoryStream = new();
        binaryWriter = new(memoryStream);

        this.channel = channel;
    }

    public void Request(long playerId)
    {
        binaryWriter.WriteShort(CommandCode);
        binaryWriter.WriteLong(playerId);

        byte[] requestData = memoryStream.ToArray();

        Debug.Log(
            "[PlayerEntitiesRequest] Sending player entities request for Player ID: " + playerId
        );
        channel.Raise(requestData);
    }
}
