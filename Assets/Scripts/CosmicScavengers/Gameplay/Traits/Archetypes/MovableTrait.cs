using CosmicScavengers.Core.Systems.Base.Traits.Data;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds selection capability to a GameObject.
    /// Following the Trait pattern allows entities to be composed of different
    /// functional blocks (Selection, Movement, Combat) independently.
    /// </summary>
    public class MovableTrait : BaseTrait
    {
        private const string DATA_KEY = "data";

        /// <summary>
        /// Local data structure matching the server's JSON format.
        /// </summary>
        [System.Serializable]
        public struct MovableSettings
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

        [SerializeField]
        private bool isMoving;

        protected override void OnInitialize()
        {
            if (
                Config.TryGetValue(DATA_KEY, out JToken dataToken) && dataToken is JObject dataBlock
            )
            {
                settings = dataBlock.ToObject<MovableSettings>();
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

        public override void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
