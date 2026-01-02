using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Extensions;
using CosmicScavengers.Core.Networking.Request.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Binary
{
    public class PlayerEntitiesRequest : BaseBinaryRequest
    {
        protected override NetworkBinaryCommand Command =>
            NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_C;

        public void Execute(object[] parameters)
        {
            if (parameters.Length < 1 || parameters[0] is not long playerId)
            {
                Debug.LogError(
                    "[PlayerEntitiesRequest] Invalid parameters for Execute. Expected Player ID (long)."
                );
                return;
            }

            Writer.WriteShort((short)Command);
            Writer.WriteLong(playerId);

            Debug.Log(
                "[PlayerEntitiesRequest] Sending player entities request for Player ID: " + playerId
            );

            Raise();
        }
    }
}
