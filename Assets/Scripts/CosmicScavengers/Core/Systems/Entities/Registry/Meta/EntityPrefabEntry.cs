using System;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Registry.Meta
{
    /// <summary>
    /// Represents a mapping between a unique string key and a corresponding Prefab.
    /// Used in the EntityRegistry to define available entity types.
    /// </summary>
    [Serializable]
    public struct EntityPrefabEntry
    {
        public string typeKey;
        public GameObject prefab;
    }
}
