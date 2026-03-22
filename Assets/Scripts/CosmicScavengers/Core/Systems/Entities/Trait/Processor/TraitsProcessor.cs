using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base;
using CosmicScavengers.Core.Systems.Entities.Traits.Meta;
using CosmicScavengers.Core.Systems.Entities.Traits.Service;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Processor
{
    public class TraitsProcessor : MonoBehaviour, ITraitsProcessor
    {
        private const int TRAIT_COUNT = 512;
        private const int PENDING_TRAIT_COUNT = 128;

        [Header("Configuration")]
        [Tooltip("Services for managing entities.")]
        [SerializeField]
        private TraitsService traitsServices;

        [SerializeField]
        private List<GameObject> gameSystems;
        private readonly Dictionary<Type, IGameSystem> systemLookUp = new();

        private readonly List<ITrait> highPriorityTraits = new(TRAIT_COUNT);
        private readonly List<ITrait> throttledTraits = new(TRAIT_COUNT);
        private readonly List<ITrait> pendingSyncTraits = new(PENDING_TRAIT_COUNT);

        private int frameCount;

        void Awake()
        {
            if (traitsServices == null)
            {
                Debug.LogError("[TraitsProcessor] TraitsService reference is missing!");
            }

            if (gameSystems == null)
            {
                Debug.LogError("[TraitsProcessor] GameSystems reference is missing!");
                return;
            }
            SetUpSystems();
        }

        private void SetUpSystems()
        {
            if (gameSystems.Count == 0)
            {
                Debug.LogWarning("[TraitsProcessor] GameSystems list is empty.");
                return;
            }

            foreach (var gameSystem in gameSystems)
            {
                if (!gameSystem.TryGetComponent<IGameSystem>(out var system))
                {
                    Debug.LogError(
                        $"[TraitsProcessor] GameSystem '{gameSystem.name}' does not implement IGameSystem."
                    );
                    continue;
                }
                systemLookUp.Add(system.GetType(), system);
            }
        }

        /// <summary>
        /// Registers a collection of traits into the appropriate update bucket.
        /// </summary>
        public void Register(IEnumerable<ITrait> traits)
        {
            if (traits == null)
            {
                Debug.LogError("[TraitsProcessor] Attempted to register null traits.");
                return;
            }

            foreach (var trait in traits)
            {
                Register(trait);
            }
        }

        /// <summary>
        /// Registers a trait into the appropriate update bucket.
        /// </summary>
        public void Register(ITrait trait)
        {
            if (trait == null)
            {
                Debug.LogError("[TraitsProcessor] Attempted to register a null trait.");
                return;
            }
            if (trait.UpdateFrequency <= 1)
            {
                if (highPriorityTraits.Contains(trait))
                {
                    Debug.LogWarning(
                        $"[TraitProcessor] Trait '{trait.Name}' already registered in high priority bucket."
                    );
                    return;
                }
                highPriorityTraits.Add(trait);
            }
            else
            {
                if (throttledTraits.Contains(trait))
                {
                    Debug.LogWarning(
                        $"[TraitProcessor] Trait '{trait.Name}' already registered in throttled bucket."
                    );
                    return;
                }
                throttledTraits.Add(trait);
            }

            Type systemType = trait.GetSystemType();
            if (systemLookUp.TryGetValue(systemType, out var system))
            {
                system.Register(trait);
            }
            else
            {
                Debug.LogError(
                    $"[TraitProcessor] Trait '{trait.Name}' did not found a matching IGameSystem Type '{systemType.Name}'."
                );
            }

            trait.OnRegister();
        }

        /// <summary>
        /// Removes a trait from the update cycle.
        /// </summary>
        public void Unregister(ITrait trait)
        {
            if (trait == null)
            {
                Debug.LogError("[TraitsProcessor] Attempted to unregister a null trait.");
                return;
            }
            if (highPriorityTraits.Contains(trait))
            {
                highPriorityTraits.Remove(trait);
            }
            else
            {
                Debug.LogWarning(
                    $"[TraitProcessor] Trait '{trait.Name}' not found in high priority bucket."
                );
                return;
            }
            if (throttledTraits.Contains(trait))
            {
                throttledTraits.Remove(trait);
            }
            else
            {
                Debug.LogWarning(
                    $"[TraitProcessor] Trait '{trait.Name}' not found in throttled bucket."
                );
            }
        }

        public void Unregister(IEnumerable<ITrait> traits)
        {
            throw new NotImplementedException();
        }

        public void RequestSync(IEnumerable<ITrait> traits)
        {
            foreach (var trait in traits)
            {
                RequestSync(trait);
            }
        }

        public void RequestSync(ITrait trait)
        {
            if (trait.IsPendingSync)
            {
                pendingSyncTraits.Add(trait);
            }
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;
            frameCount++;

            // High-Priority (Every Frame)
            foreach (var trait in highPriorityTraits)
            {
                trait.PendingUpdate = true;
                if (trait.IsPendingSync)
                {
                    pendingSyncTraits.Add(trait);
                }
            }

            // Throttled (Only if it's their turn)
            foreach (var trait in throttledTraits)
            {
                trait.PendingUpdate = frameCount % trait.UpdateFrequency == 0;
                if (trait.IsPendingSync)
                {
                    pendingSyncTraits.Add(trait);
                }
            }

            // Low-Priority (Every Frame)

            foreach (var system in systemLookUp.Values)
            {
                system.OnTick(deltaTime);
            }
        }

        /// <summary>
        /// The Synchronization Phase.
        /// Hand off all collected 'Intents' to the TraitsService for network dispatch.
        /// </summary>
        private void LateUpdate()
        {
            int syncCount = pendingSyncTraits.Count;
            if (syncCount == 0)
            {
                return;
            }

            for (int i = 0; i < syncCount; i++)
            {
                ITrait trait = pendingSyncTraits[i];
                if (traitsServices != null)
                {
                    traitsServices.RequestTraitSync(trait.Owner, trait);
                }
                trait.ClearSync();
            }
            pendingSyncTraits.Clear();
        }
    }
}
