using CosmicScavengers.Core.Systems.Entities.Meta;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace CosmicScavengers.Core.Systems.Traits.Data.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// </summary>
    public interface ITrait
    {
        string Name { get; set; }
        IEntity Owner { get; }
        int UpdateFrequency { get; }
        void Initialize(IEntity owner, JObject data);
        void OnUpdate(float deltaTime);
    }
}
