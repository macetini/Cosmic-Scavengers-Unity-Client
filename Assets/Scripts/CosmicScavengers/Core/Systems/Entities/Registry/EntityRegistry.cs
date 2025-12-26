using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Entities.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Registry
{
    [CreateAssetMenu(menuName = "Registry/EntityRegistry")]
    public class EntityRegistry : ScriptableObject
    {
        public List<EntityEntry> Entries = new();
        private readonly Dictionary<string, BaseEntity> lookUp = new();

        public BaseEntity GetPrefab(string key)
        {
            if (lookUp.Count == 0)
            {
                InitializeLookup();
            }
            return lookUp.TryGetValue(key.Trim().ToUpper(), out var prefab) ? prefab : null;
        }

        private void InitializeLookup()
        {
            lookUp.Clear();
            foreach (var entry in Entries)
            {
                if (!string.IsNullOrEmpty(entry.Key))
                {
                    lookUp[entry.Key.Trim().ToUpper()] = entry.Prefab;
                }
            }
        }
    }
}
