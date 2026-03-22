using CosmicScavengers.Core.Extensions;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Request.Data.Binary;
using CosmicScavengers.Networking.Protobuf.Traits;
using Google.Protobuf;
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
        /// <param name="parameters">Expects [MoveIntentProto protoMessage].</param>
        protected override bool PackParameters(object[] parameters)
        {
            if (parameters == null || parameters.Length != 2)
            {
                Debug.LogError(
                    $"[{name}] Wrong number of parameters: {parameters.Length}. Expecting 2 parameter: [long EntityId, {MoveIntentProto.Descriptor.FullName}]."
                );
                return false;
            }

            if (parameters[1] is not MoveIntentProto protoMessage)
            {
                Debug.LogError(
                    $"[{name}] Parameter type mismatch. Check your RequestChannel.Raise call."
                );
                return false;
            }

            byte[] protoBytes = protoMessage.ToByteArray();
            Writer.WriteInt(protoBytes.Length);
            Writer.Write(protoBytes);

            Debug.Log(
                $"[{name}] Packed 'MoveEntityRequest' Request for Entity {protoMessage.EntityId} with Scaled Target Value: [X: {protoMessage.RequestData.TargetX}, Y: {protoMessage.RequestData.TargetY}, Z: {protoMessage.RequestData.TargetZ}]"
            );

            return true;
        }
    }
}
