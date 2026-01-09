using System;
using CosmicScavengers.Core.Systems.Entity.Traits;

namespace CosmicScavengers.Core.Systems.Entities.Traits.Registry.Meta
{
    [Serializable]
    public class TraitEntry
    {
        public string Key;
        public BaseTrait Prefab;
    }
}
