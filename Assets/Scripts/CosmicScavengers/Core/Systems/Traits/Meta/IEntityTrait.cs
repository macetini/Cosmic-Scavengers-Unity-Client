using CosmicScavengers.Core.Systems.Data.Entities;

namespace CosmicScavengers.Core.Systems.Traits.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// </summary>
    public interface IEntityTrait
    {
        string Name { get; set;}
        void Initialize(BaseEntity owner);
        void OnUpdate(float deltaTime);
    }
}
