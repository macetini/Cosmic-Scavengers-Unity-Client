using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Traits.Meta;
using CosmicScavengers.Core.Systems.Entities.Traits.Service;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Processor
{
    public class TraitsProcessor : MonoBehaviour, ITraitsProcessor
    {
        [Tooltip("Services for managing entities.")]
        [SerializeField]
        private TraitsService traitsServices;

        private const int TRAIT_COUNT = 512;
        private const int PENDING_TRAIT_COUNT = 128;
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

        public void Register(IEnumerable<ITrait> traits)
        {
            throw new System.NotImplementedException();
        }

        public void Unregister(IEnumerable<ITrait> traits)
        {
            throw new System.NotImplementedException();
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

            // Process High-Priority (Every Frame)
            int highCount = highPriorityTraits.Count;
            for (int i = 0; i < highCount; i++)
            {
                highPriorityTraits[i].OnUpdate(deltaTime);
            }

            // Process Throttled (Every Nth Frame)
            frameCount++;
            int throttledCount = throttledTraits.Count;
            for (int i = 0; i < throttledCount; i++)
            {
                var trait = throttledTraits[i];
                if (frameCount % trait.UpdateFrequency == 0)
                {
                    // Normalize delta time so logic remains consistent regardless of frequency
                    if (trait.Active)
                    {
                        trait.OnUpdate(deltaTime * trait.UpdateFrequency);
                    }
                }
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
