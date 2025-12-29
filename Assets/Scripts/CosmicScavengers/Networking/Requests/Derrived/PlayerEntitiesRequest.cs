using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Extensions;
using CosmicScavengers.Core.Networking.Request.Data;
using UnityEngine;

public class PlayerEntitiesRequest : BaseBinaryRequest
{
    public override NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_C;

    public override void Execute(params object[] parameters)
    {
        if (parameters.Length < 1 || parameters[0] is not long playerId)
        {
            Debug.LogError(
                "[PlayerEntitiesRequest] Invalid parameters for Execute. Expected Player ID (long)."
            );
            return;
        }

        playerId = (long)parameters[0];

        Writer.WriteShort((short)Command);
        Writer.WriteLong(playerId);

        byte[] requestData = Stream.ToArray();

        Debug.Log(
            "[PlayerEntitiesRequest] Sending player entities request for Player ID: " + playerId
        );

        CommandChannel.Raise(requestData);
    }
}
