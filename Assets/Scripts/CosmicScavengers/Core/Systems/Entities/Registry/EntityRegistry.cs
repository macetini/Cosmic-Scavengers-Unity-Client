using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Registry
{
    [CreateAssetMenu(menuName = "Registry/EntityRegistry")]
    public class EntityRegistry : ScriptableObject
    {
        [SerializeField]
        private List<EntityPrefabEntry> entries;

        private Dictionary<string, GameObject> lookUp;

        public GameObject GetPrefab(string key)
        {
            if (lookUp == null)
            {
                lookUp = new Dictionary<string, GameObject>();
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.typeKey))
                    {
                        lookUp[entry.typeKey] = entry.prefab;
                    }
                }
            }

            return lookUp.TryGetValue(key, out var prefab) ? prefab : null;
        }
    }
}
