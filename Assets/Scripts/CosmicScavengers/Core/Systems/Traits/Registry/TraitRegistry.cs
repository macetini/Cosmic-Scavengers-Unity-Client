using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Registry
{
    [CreateAssetMenu(menuName = "Registry/TraitRegistry")]
    public class TraitRegistry : ScriptableObject
    {
        [System.Serializable]
        public struct TraitEntry
        {
            public string TraitKey; // e.g., "MOVABLE", "SELECTABLE"
            public BaseTrait Prefab; // The Trait Prefab with the Script
        }

        public List<TraitEntry> Registry = new();

        public BaseTrait GetPrefab(string key)
        {
            var entry = Registry.Find(e => e.TraitKey == key);
            return entry.Prefab;
        }
    }
}
