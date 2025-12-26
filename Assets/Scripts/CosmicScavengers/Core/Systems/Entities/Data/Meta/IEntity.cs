using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base.Traits.Data;

namespace CosmicScavengers.Core.Systems.Entities.Meta
{
    public interface IEntity
    {
        long Id { get; set; }
        string Type { get; set; }
        bool IsStatic { get; set; }
        List<BaseTrait> Traits { get; set; }
        void OnSpawned();
        void OnRemoved();
    }
}
