using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Core.Systems.Utils.Scale4f;
using CosmicScavengers.Networking.Protobuf.Traits;
using Google.Protobuf;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Entities.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds movement capability to a GameObject.
    /// Handles both the intent dispatch to the server and the visual simulation.
    /// </summary>
    public class MovableTrait : BaseTrait
    {
        private const string DATA_KEY = "data"; // TODO - Separate to config (investigate, what is the best way to do this?)

        private MovableTraitProto traitData = new()
        {
            Status = MovableTraitProto.Types.Status.Idle,
            //Data = new MovementDataProto(),
        };

        [Header("Interpolation")]
        [Tooltip("How fast the visual transform snaps to the target rotation.")]
        [SerializeField]
        private float rotationLerpModifier = 5f;

        [Header("Movement State")]
        [SerializeField]
        private Vector3 targetPosition;

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
                /*RequestData = new MovementDataProto
                {
                    TargetX = DeterministicUtils.ToScaled(targetPosition.x),
                    TargetY = DeterministicUtils.ToScaled(targetPosition.y),
                    TargetZ = DeterministicUtils.ToScaled(targetPosition.z),

                    MovementSpeed = traitData.Data.MovementSpeed,
                    RotationSpeed = traitData.Data.RotationSpeed,
                    StoppingDistance = traitData.Data.StoppingDistance,
                },*/
            };

            return new object[] { intent };
        }

        protected override void OnInitialize()
        {
            if (Config.TryGetValue(DATA_KEY, out JToken dataToken))
            {
                var parser = new JsonParser(
                    JsonParser.Settings.Default.WithIgnoreUnknownFields(true)
                );

                traitData = parser.Parse<MovableTraitProto>(dataToken.ToString());
                /*targetPosition = new Vector3(
                    DeterministicUtils.FromScaled(traitData.Data.TargetX),
                    DeterministicUtils.FromScaled(traitData.Data.TargetY),
                    DeterministicUtils.FromScaled(traitData.Data.TargetZ)
                );*/
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

            traitData.Status = MovableTraitProto.Types.Status.Moving;

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
