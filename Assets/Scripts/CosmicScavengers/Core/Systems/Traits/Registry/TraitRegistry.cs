using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base.Traits.Data;
using CosmicScavengers.Core.Systems.Traits.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Registry
{
    [CreateAssetMenu(menuName = "Registry/TraitRegistry")]
    public class TraitRegistry : ScriptableObject
    {
        public List<TraitEntry> Entries = new();
        private readonly Dictionary<string, BaseTrait> lookUp = new();

        public BaseTrait GetPrefab(string key)
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
