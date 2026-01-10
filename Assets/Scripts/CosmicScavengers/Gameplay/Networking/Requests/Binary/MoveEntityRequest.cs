using CosmicScavengers.Core.Extensions;
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
            if (parameters == null || parameters.Length != 5)
            {
                Debug.LogError(
                    $"[{name}] Missing parameters. Expecting 5 parameters: [long entityId, Vector3 targetPosition, float movementSpeed, float rotationSpeed, float stoppingDistance]."
                );
                return false;
            }

            if (
                parameters[0] is not long entityId
                || parameters[1] is not Vector3 position
                || parameters[2] is not float
                || parameters[3] is not float
                || parameters[4] is not float
            )
            {
                Debug.LogError(
                    $"[{name}] Parameter type mismatch. Check your RequestChannel.Raise call."
                );
                return false;
            }

            Writer.WriteLong(entityId);

            Writer.WriteFloat(position.x);
            Writer.WriteFloat(position.y);
            Writer.WriteFloat(position.z);

            Writer.WriteFloat((float)parameters[2]); // MovementSpeed
            Writer.WriteFloat((float)parameters[3]); // RotationSpeed
            Writer.WriteFloat((float)parameters[4]); // StoppingDistance

            Debug.Log($"[Network] Packed Move Request for Entity {entityId} to {position}");

            return true;
        }
    }
}
