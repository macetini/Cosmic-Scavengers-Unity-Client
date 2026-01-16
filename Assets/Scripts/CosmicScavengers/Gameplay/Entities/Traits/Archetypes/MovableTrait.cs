using System;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Core.Systems.Utils.Scale4f;
using Unity.Plastic.Newtonsoft.Json;
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
        private const string DATA_KEY = "data";

        /// <summary>
        /// Local data structure matching the server's JSON format.
        /// Includes the 'status' and 'target' fields for authoritative state.
        /// </summary>
        [Serializable]
        private struct MovableSettings
        {
            public enum StatusType
            {
                IDLE,
                MOVING,
            }

            [JsonProperty("movement_speed")]
            public float MovementSpeed;

            [JsonProperty("rotation_speed")]
            public float RotationSpeed;

            [JsonProperty("stopping_distance")]
            public float StoppingDistance;

            [JsonProperty("status")]
            public StatusType CurrentStatus; // "IDLE" or "MOVING"

            // Target coordinates stored as scaled longs in JSON for precision
            [JsonProperty("target_x")]
            public long TargetX;

            [JsonProperty("target_y")]
            public long TargetY;

            [JsonProperty("target_z")]
            public long TargetZ;

            /// <summary>
            /// Returns a settings object with sensible default values.
            /// </summary>
            public static MovableSettings Default() =>
                new()
                {
                    CurrentStatus = StatusType.IDLE,
                    TargetX = -1,
                    TargetY = -1,
                    TargetZ = -1,
                };
        }

        [Header("Runtime Configuration")]
        [SerializeField]
        private MovableSettings settings = MovableSettings.Default();

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

        /// <summary>
        /// Prepares the payload for the MoveEntityRequest.
        /// Pre-pends the Owner.Id via the TraitsService.
        /// </summary>
        public override object[] GetSyncPayload()
        {
            return new object[]
            {
                targetPosition,
                settings.MovementSpeed,
                settings.RotationSpeed,
                settings.StoppingDistance,
            };
        }

        /// <summary>
        /// Called when the entity is spawned or when state_data is updated from the server.
        /// </summary>
        protected override void OnInitialize()
        {
            if (
                Config.TryGetValue(DATA_KEY, out JToken dataToken) && dataToken is JObject dataBlock
            )
            {
                settings = dataBlock.ToObject<MovableSettings>();

                // Convert authoritative scaled coordinates to Unity World Space
                targetPosition = new Vector3(
                    DeterministicUtils.FromScaled(settings.TargetX),
                    DeterministicUtils.FromScaled(settings.TargetY),
                    DeterministicUtils.FromScaled(settings.TargetZ)
                );

                // If the server says we are moving, ensure the trait is active for OnUpdate
                if (settings.CurrentStatus == MovableSettings.StatusType.MOVING)
                {
                    Active = true;
                }

                Debug.Log(
                    $"[{Name}] Initialized: Speed={settings.MovementSpeed}, StopDist={settings.StoppingDistance}"
                );
            }
            else
            {
                Debug.LogWarning(
                    $"[{Name}] No '{DATA_KEY}' block found in configuration for Entity {Owner?.Id}. Using defaults."
                );
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

            if (settings.CurrentStatus != MovableSettings.StatusType.MOVING)
            {
                return;
            }

            // 1. Visual Position Update
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                settings.MovementSpeed * deltaTime
            );

            // 2. Update Visual Rotation
            Vector3 diff = targetPosition - transform.position;

            if (diff.sqrMagnitude > 0.001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(diff.normalized);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    lookRotation,
                    deltaTime * rotationLerpModifier
                );
            }

            // 3. Client-side Prediction of Arrival
            // We use stopping distance to stop the visual loop.
            // The server will eventually send a state update where Status == IDLE.
            if (Vector3.Distance(transform.position, targetPosition) <= settings.StoppingDistance)
            {
                settings.CurrentStatus = MovableSettings.StatusType.IDLE;
            }
        }
    }
}
