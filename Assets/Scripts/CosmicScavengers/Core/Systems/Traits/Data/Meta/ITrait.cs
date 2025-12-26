using CosmicScavengers.Core.Systems.Data.Entities;

namespace CosmicScavengers.Core.Systems.Traits.Data.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// </summary>
    public interface ITrait
    {
        string Name { get; set; }
        int UpdateFrequency { get; }
        void Initialize(BaseEntity owner);
        void OnUpdate(float deltaTime);
    }
}
