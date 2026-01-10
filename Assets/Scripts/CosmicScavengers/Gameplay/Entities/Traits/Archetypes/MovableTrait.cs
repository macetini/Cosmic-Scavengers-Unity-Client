using System;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entity.Traits;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Entities.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds movement capability to a GameObject.
    /// </summary>
    public class MovableTrait : BaseTrait
    {
        private const string DATA_KEY = "data";

        /// <summary>
        /// Local data structure matching the server's JSON format.
        /// </summary>
        [Serializable]
        private struct MovableSettings
        {
            [JsonProperty("movement_speed")]
            public float MovementSpeed;

            [JsonProperty("rotation_speed")]
            public float RotationSpeed;

            [JsonProperty("stopping_distance")]
            public float StoppingDistance;
        }

        [Header("Runtime Configuration")]
        [SerializeField]
        private MovableSettings settings;

        [Header("Movement State")]
        [SerializeField]
        private Vector3 targetPosition;

        public override BaseNetworkCommand GetSyncCommand()
        {
            return NetworkBinaryCommand.REQUEST_ENTITY_MOVE_C;
        }

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

        protected override void OnInitialize()
        {
            if (
                Config.TryGetValue(DATA_KEY, out JToken dataToken) && dataToken is JObject dataBlock
            )
            {
                settings = dataBlock.ToObject<MovableSettings>();
                targetPosition = Vector3.zero;

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

        public override void OnUpdate(float deltaTime) { }
    }
}
