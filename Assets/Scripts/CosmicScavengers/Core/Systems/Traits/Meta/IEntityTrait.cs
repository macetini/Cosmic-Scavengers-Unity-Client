using CosmicScavengers.Core.Systems.Entities;

namespace CosmicScavengers.Core.Systems.Traits.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// </summary>
    public interface IEntityTrait
    {
        string Name { get; set;}
        void Initialize(EntityBase owner);
        void OnUpdate(float deltaTime);
    }
}
