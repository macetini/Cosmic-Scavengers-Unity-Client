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
                || parameters[2] is not long movementSpeed
                || parameters[3] is not long rotationSpeed
                || parameters[4] is not long stoppingDistance
            )
            {
                Debug.LogError(
                    $"[{name}] Parameter type mismatch. Check your RequestChannel.Raise call."
                );
                return false;
            }

            long scaledX = DeterministicUtils.ToScaled(position.x);
            long scaledY = DeterministicUtils.ToScaled(position.y);
            long scaledZ = DeterministicUtils.ToScaled(position.z);

            Writer.WriteLong(entityId);

            Writer.WriteLong(scaledX);
            Writer.WriteLong(scaledY);
            Writer.WriteLong(scaledZ);

            Writer.WriteLong(movementSpeed);
            Writer.WriteLong(rotationSpeed);
            Writer.WriteLong(stoppingDistance);

            Debug.Log(
                $"[Network] Packed Move Request for Entity {entityId} to with scaled values Target: [X:{scaledX}, Y:{scaledY}, Z:{scaledZ}] - Movement Speed: [{movementSpeed}], Rotation Speed: [{rotationSpeed}], Stopping Distance: [{stoppingDistance}]"
            );

            return true;
        }
    }
}
