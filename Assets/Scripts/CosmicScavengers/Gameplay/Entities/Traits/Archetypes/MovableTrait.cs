using System;
using System.ComponentModel;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entities.Movement;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Core.Systems.Utils.Scale4f;
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
        [Header("Movement State")]
        [Tooltip("The target position to move to.")]
        [SerializeField, ReadOnly(true)]
        private Vector3 targetPosition;
        public Vector3 TargetPosition
        {
            get => targetPosition;
            private set => targetPosition = value;
        }

        private MovableTraitProto data;

        public override Type GetSystemType()
        {
            return typeof(MovementSystem);
        }

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
                RequestData = new MoveTargetProto()
                {
                    TargetX = DeterministicUtils.ToScaled(TargetPosition.x),
                    TargetY = DeterministicUtils.ToScaled(TargetPosition.y),
                    TargetZ = DeterministicUtils.ToScaled(TargetPosition.z),
                },
            };
            return new object[] { intent };
        }

        protected override void InitializeData()
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

        public override void OnRegister()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Issues a new move instruction.
        /// This marks the trait as 'PendingSync', which the EntityOrchestrator
        /// will detect and dispatch to the server.
        /// </summary>
        public void IssueMoveOrder(Vector3 destination)
        {
            Debug.Log($"[MovableTrait] IssueMoveOrder to {destination}");
            TargetPosition = destination;
            Active = true;
            RequestSync();
        }
    }
}
