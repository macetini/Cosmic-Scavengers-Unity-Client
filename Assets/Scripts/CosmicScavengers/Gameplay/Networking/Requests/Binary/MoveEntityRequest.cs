using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Request.Data.Binary;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Requests.Derived.Binary
{
    public class MoveEntityRequest : BaseBinaryRequest
    {
        protected override NetworkBinaryCommand Command =>
            NetworkBinaryCommand.REQUEST_ENTITY_MOVE_C;

        /// <summary>
        /// Packs the movement data into the binary stream.
        /// </summary>
        /// <param name="parameters">Expects [long entityId, Vector3 targetPosition].</param>
        protected override bool PackParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogError(
                    $"[{name}] Missing parameters. Expected (long)ID and (Vector3)Target."
                );
                return false;
            }

            if (parameters[0] is not long entityId || parameters[1] is not Vector3 position)
            {
                Debug.LogError(
                    $"[{name}] Parameter type mismatch. Check your RequestChannel.Raise call."
                );
                return false;
            }

            Writer.Write(entityId);

            Writer.Write(position.x);
            Writer.Write(position.y);
            Writer.Write(position.z);

            Debug.Log($"[Network] Packed Move Request for Entity {entityId} to {position}");

            return true;
        }
    }
}
