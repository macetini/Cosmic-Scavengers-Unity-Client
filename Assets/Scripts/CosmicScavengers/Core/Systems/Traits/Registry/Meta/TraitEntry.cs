using System;
using CosmicScavengers.Core.Systems.Base.Traits.Data;

namespace CosmicScavengers.Core.Systems.Traits.Registry.Meta
{
    [Serializable]
    public class TraitEntry
    {
        public string Key;
        public BaseTrait Prefab;
    }
}
