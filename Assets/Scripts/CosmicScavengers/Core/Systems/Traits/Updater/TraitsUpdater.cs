using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base.Traits;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Updater
{
    public class TraitsUpdater : MonoBehaviour
    {
        private readonly List<BaseTrait> highPriorityTraits = new();
        private readonly List<BaseTrait> throttledTraits = new();
        private int frameCount;

        /// <summary>
        /// Registers a trait into the appropriate update bucket.
        /// </summary>
        public void Register(BaseTrait trait)
        {
            if (trait.UpdateFrequency <= 1)
            {
                highPriorityTraits.Add(trait);
            }
            else
            {
                throttledTraits.Add(trait);
            }
        }

        /// <summary>
        /// Removes a trait from the update cycle.
        /// </summary>
        public void Unregister(BaseTrait trait)
        {
            highPriorityTraits.Remove(trait);
            throttledTraits.Remove(trait);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            frameCount++;

            // 1. Process High-Priority (Every Frame)
            // We use a standard for-loop to avoid iterator allocation overhead in the hot path.
            int highCount = highPriorityTraits.Count;
            for (int i = 0; i < highCount; i++)
            {
                highPriorityTraits[i].OnUpdate(dt);
            }

            // 2. Process Throttled (Every Nth Frame)
            // This spreads the CPU load of heavy traits across multiple frames.
            int throttledCount = throttledTraits.Count;
            for (int i = 0; i < throttledCount; i++)
            {
                var trait = throttledTraits[i];
                if (frameCount % trait.UpdateFrequency == 0)
                {
                    // Normalize delta time so logic remains consistent regardless of frequency
                    trait.OnUpdate(dt * trait.UpdateFrequency);
                }
            }
        }
    }
}
