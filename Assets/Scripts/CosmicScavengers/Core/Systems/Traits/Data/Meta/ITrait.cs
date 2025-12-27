using CosmicScavengers.Core.Systems.Entities.Meta;

namespace CosmicScavengers.Core.Systems.Traits.Data.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// </summary>
    public interface ITrait
    {
        string Name { get; set; }
        int UpdateFrequency { get; }
        void Initialize(IEntity owner);
        void OnUpdate(float deltaTime);
    }
}
