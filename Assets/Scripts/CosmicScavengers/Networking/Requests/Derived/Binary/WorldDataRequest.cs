using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Extensions;
using CosmicScavengers.Networking.Request.Binary.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Binary
{
    public class WorldStateRequest : BaseBinaryRequest
    {
        protected override NetworkBinaryCommand Command =>
            NetworkBinaryCommand.REQUEST_WORLD_STATE_C;

        public void Execute(object[] parameters)
        {
            if (parameters.Length < 1 || parameters[0] is not long playerId)
            {
                Debug.LogError(
                    "[PlayerEntitiesRequest] Invalid parameters for Execute. Expected Player ID (long)."
                );
                return;
            }

            Writer.WriteLong(playerId);

            Debug.Log(
                "[PlayerEntitiesRequest] Sending world state request for Player ID: " + playerId
            );

            Raise();
        }
    }
}
