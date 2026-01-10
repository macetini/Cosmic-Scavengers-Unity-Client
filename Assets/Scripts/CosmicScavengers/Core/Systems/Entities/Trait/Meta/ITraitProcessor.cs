using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;

namespace CosmicScavengers.Core.Systems.Entities.Traits.Meta
{
    /// <summary>
    /// The contract for the engine that processes and synchronizes traits.
    /// Resides in the Traits.Meta namespace to keep all trait-related contracts together.
    /// </summary>
    public interface ITraitsProcessor
    {
        /// <summary>
        /// Registers a specific trait into the simulation buckets.
        /// </summary>
        public void Register(ITrait trait);

        /// <summary>
        /// Registers a collection of traits into the simulation buckets.
        /// </summary>
        void Register(IEnumerable<ITrait> traits);

        /// <summary>
        /// Removes a specific trait from the simulation buckets.
        /// </summary>
        public void Unregister(ITrait trait);

        /// <summary>
        /// Removes a collection of traits from the simulation buckets.
        /// </summary>
        void Unregister(IEnumerable<ITrait> traits);

        /// <summary>
        /// Stages a specific trait for synchronization in the next network tick.
        /// </summary>
        /// <param name="traits">Trait that will be synced</param>
        void RequestSync(ITrait trait);

        /// <summary>
        ///  Stages a specific traits for synchronization in the next network tick.
        /// </summary>
        /// <param name="traits"></param>
        void RequestSync(IEnumerable<ITrait> traits);
    }
}
