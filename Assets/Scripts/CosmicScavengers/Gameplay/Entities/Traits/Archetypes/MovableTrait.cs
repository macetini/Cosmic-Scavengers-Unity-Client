using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Networking.Protobuf.Traits;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Entities.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds movement capability to a GameObject.
    /// Handles both the intent dispatch to the server and the visual simulation.
    /// </summary>
    public class MovableTrait : BaseTrait
    {
        [Header("Interpolation")]
        [Tooltip("How fast the visual transform snaps to the target rotation.")]
        [SerializeField]
        private readonly float rotationLerpModifier = 5f;

        [Header("Movement State")]
        [SerializeField]
        private Vector3 targetPosition;

        private MovableTraitProto data;

        /// <summary>
        /// The binary command used to request a move on the server.
        /// </summary>
        public override BaseNetworkCommand GetSyncCommand()
        {
            return NetworkBinaryCommand.REQUEST_ENTITY_MOVE_C;
        }

        public override object[] GetSyncPayload()
        {
            MoveIntentProto intent = new()
            {
                EntityId = Owner.Id,
                //RequestData = new MoveIntentDataProto()
            };
            return new object[] { intent };
        }

        protected override void Initialize()
        {
            data = protoData as MovableTraitProto;
            if (data == null)
            {
                Debug.LogError(
                    $"[MovableTrait] Failed to cast ProtoData for entity {Owner?.Id}. Expected MovableTraitProto."
                );
                return;
            }
        }

        /// <summary>
        /// Issues a new move instruction.
        /// This marks the trait as 'PendingSync', which the EntityOrchestrator
        /// will detect and dispatch to the server.
        /// </summary>
        public void IssueMoveOrder(Vector3 destination)
        {
            Debug.Log($"[MovableTrait] IssueMoveOrder to {destination}");
            targetPosition = destination;

            Active = true;

            RequestSync();
        }

        /// <summary>
        /// Performed by the TraitProcessor.
        /// Handles the visual movement of the entity toward the authoritative target.
        /// </summary>
        public override void OnUpdate(float deltaTime)
        {
            if (!Active)
            {
                Debug.LogError("[MovableTrait] Attempted to update an inactive trait.");
                return;
            }

            // Use DeterministicUtils to turn the "Big Ints" into "Unity Floats"
            /*
            float visualSpeed = DeterministicUtils.FromScaled(traitData.Data.MovementSpeed);

            // 1. Visual Position Update
            Owner.Transform.position = Vector3.MoveTowards(
                Owner.Transform.position,
                targetPosition,
                visualSpeed * deltaTime
            );

            // 2. Update Visual Rotation
            Vector3 diff = targetPosition - Owner.Transform.position;
            if (diff.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(diff.normalized);
                Owner.Transform.rotation = Quaternion.Slerp(
                    Owner.Transform.rotation,
                    lookRotation,
                    deltaTime * rotationLerpModifier
                );
            }

            float visualStopDist = DeterministicUtils.FromScaled(traitData.Data.StoppingDistance);
            bool arrived =
                Vector3.Distance(Owner.Transform.position, targetPosition) <= visualStopDist;
            if (arrived)
            {
                traitData.Status = MovableTraitProto.Types.Status.Idle;
                Active = false;
            }*/
        }
    }
}
