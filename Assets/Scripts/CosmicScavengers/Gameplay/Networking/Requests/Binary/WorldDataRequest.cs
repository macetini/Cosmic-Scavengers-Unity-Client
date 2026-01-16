using CosmicScavengers.Core.Extensions;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Request.Data.Binary;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Requests.Derived.Binary
{
    public class WorldStateRequest : BaseBinaryRequest
    {
        protected override NetworkBinaryCommand Command =>
            NetworkBinaryCommand.REQUEST_PLAYER_INIT_STATE_C;

        protected override bool PackParameters(object[] parameters)
        {
            if (parameters.Length < 1 || parameters[0] is not long playerId)
            {
                Debug.LogError(
                    "[WorldStateRequest] Invalid parameters for Execute. Expected Player ID (long)."
                );
                return false;
            }

            Writer.WriteLong(playerId);

            Debug.Log("[WorldStateRequest] Sending world state request for Player ID: " + playerId);

            return true;
        }
    }
}
