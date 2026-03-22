using CosmicScavengers.Core.Systems.Entity.Traits.Meta;

namespace CosmicScavengers.Core.Systems.Base
{
    /// <summary>
    /// Contract for specialized logic engines that process specific trait types.
    /// </summary>
    public interface IGameSystem
    {
        /// <summary>
        /// Human-readable name for debugging and logs.
        /// </summary>
        string SystemName { get; }

        /// <summary>
        /// Called by the TraitProcessor during the main Update loop.
        /// </summary>
        void OnTick(float deltaTime);

        /// <summary>
        /// Called during LateUpdate for logic that must happen after all ticks.
        /// Useful for networking synchronization or camera smoothing.
        /// </summary>
        void OnLateTick();

        /// <summary>
        /// General registration for the TraitProcessor to hand off a trait.
        /// </summary>
        void Register(ITrait trait);

        /// <summary>
        /// General unregistration for cleanup.
        /// </summary>
        void Unregister(ITrait trait);
    }
}
