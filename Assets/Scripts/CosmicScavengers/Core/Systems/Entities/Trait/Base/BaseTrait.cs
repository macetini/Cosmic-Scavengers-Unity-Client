using System;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entity.Traits
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, ITrait
    {
        private const string PRIORITY_KEY = "priority";
        private const string UPDATE_FREQUENCY_KEY = "update_frequency";

        [Header("Visual Feedback")]
        [SerializeField]
        [Tooltip("The display name of this trait. If empty, the class name will be used.")]
        private string traitName;
        public string Name
        {
            get => string.IsNullOrEmpty(traitName) ? GetType().Name : traitName;
            set => traitName = value;
        }

        /// <summary>
        /// Indicates if the trait has a pending request (Movement, Attack, etc.)
        /// that needs to be synchronized with the server via a Service.
        /// </summary>
        public bool IsPendingSync { get; protected set; } = false;

        /// <summary>
        /// Controls whether the OnUpdate logic should run.
        /// </summary>
        public bool IsEnabled { get; } = true;

        public bool Active { get; protected set; } = true;

        public IEntity Owner { get; private set; }
        protected JObject Config;
        public int Priority { get; private set; }
        public int UpdateFrequency { get; private set; }

        /// <summary>
        /// Called by the TraitsUpdater/Service once the sync request has been dispatched.
        /// </summary>
        public void ClearSync()
        {
            IsPendingSync = false;
        }

        public void RequestSync()
        {
            IsPendingSync = true;
            Owner.RequestTraitSync(this);
        }

        public virtual BaseNetworkCommand GetSyncCommand()
        {
            return NetworkBinaryCommand.UNKNOWN;
        }

        public virtual object[] GetSyncPayload()
        {
            return null;
        }

        public void Initialize(IEntity owner, JObject config)
        {
            SetOwner(owner);
            ParseConfig(config);

            OnInitialize();
        }

        private void SetOwner(IEntity owner)
        {
            Owner =
                owner
                ?? throw new ArgumentNullException(
                    "Owner cannot be null for BaseTrait initialization."
                );
        }

        private void ParseConfig(JObject config)
        {
            Config =
                config
                ?? throw new ArgumentNullException(
                    "Config cannot be null for BaseTrait initialization."
                );

            if (
                Config.TryGetValue(PRIORITY_KEY, out JToken priorityToken)
                && Config.TryGetValue(UPDATE_FREQUENCY_KEY, out JToken updateFrequencyToken)
            )
            {
                Priority = priorityToken.Value<int>();
                UpdateFrequency = updateFrequencyToken.Value<int>();
            }
            else
            {
                throw new ArgumentException($"Missing required fields in {GetType().Name} config.");
            }
        }

        /// <summary>
        /// Optional override for subclasses to perform their own setup logic.
        /// </summary>
        protected virtual void OnInitialize() { }

        public bool OwnedBySameEntity(BaseTrait otherTrait)
        {
            return otherTrait != null
                && Owner != null
                && otherTrait.Owner != null
                && Owner.Id == otherTrait.Owner.Id;
        }

        /// <summary>
        /// The update loop managed by BaseEntity.
        /// </summary>
        public abstract void OnUpdate(float deltaTime);
    }
}
