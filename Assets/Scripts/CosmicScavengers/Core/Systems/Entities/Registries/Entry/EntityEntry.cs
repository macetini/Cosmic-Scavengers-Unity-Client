using System;
using CosmicScavengers.Core.Systems.Entities.Base;

namespace CosmicScavengers.Core.Systems.Entities.Registry.Entry
{
    /// <summary>
    /// Represents a mapping between a unique string key and a corresponding Prefab.
    /// Used in the EntityRegistry to define available entity types.
    /// </summary>
    [Serializable]
    public struct EntityEntry
    {
        public string Key;
        public BaseEntity Prefab;
    }
}
