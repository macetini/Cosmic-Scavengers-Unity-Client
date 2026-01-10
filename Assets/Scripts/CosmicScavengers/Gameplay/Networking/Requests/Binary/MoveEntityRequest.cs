using CosmicScavengers.Core.Extensions;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Request.Data.Binary;
using CosmicScavengers.Core.Systems.Utils.Scale4f;
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
                || parameters[2] is not float movementSpeed
                || parameters[3] is not float rotationSpeed
                || parameters[4] is not float stoppingDistance
            )
            {
                Debug.LogError(
                    $"[{name}] Parameter type mismatch. Check your RequestChannel.Raise call."
                );
                return false;
            }

            long scaledX = DeterministicUtils.ToUnscaled(position.x);
            long scaledY = DeterministicUtils.ToUnscaled(position.y);
            long scaledZ = DeterministicUtils.ToUnscaled(position.z);

            long scaledMovementSpeed = DeterministicUtils.ToUnscaled(movementSpeed);
            long scaledRotationSpeed = DeterministicUtils.ToUnscaled(rotationSpeed);
            long scaledStoppingDistance = DeterministicUtils.ToUnscaled(stoppingDistance);

            Writer.WriteLong(entityId);

            Writer.WriteLong(scaledX);
            Writer.WriteLong(scaledY);
            Writer.WriteLong(scaledZ);

            Writer.WriteLong(scaledMovementSpeed);
            Writer.WriteLong(scaledRotationSpeed);
            Writer.WriteLong(scaledStoppingDistance);

            Debug.Log($"[Network] Packed Move Request for Entity {entityId} to {position}");

            return true;
        }
    }
}
