using System.Collections.Generic;
using System.Linq;
using CosmicScavengers.Core.Systems.Entities;
using CosmicScavengers.Networking.Protobuf.Entities;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.Systems.Entities.Archetypes
{
    /// <summary>
    /// A generalized class for any mobile unit (Ships, Drones).
    /// Handles common unit logic like movement and actions.
    /// </summary>
    public class UnitEntity : EntityBase
    {
        /// <summary>
        /// This overrides the abstract method in EntityBase to handle
        /// the specific Protobuf message type.
        /// </summary>
        public override void UpdateState(string stateData)
        {
            if (string.IsNullOrEmpty(stateData))
            {
                Debug.LogWarning($"[UnitEntity] No StateData provided for Entity {Id}.");
                return;
            }

            JObject json = JObject.Parse(stateData);

            InitTraits(json);
        }

        private void InitTraits(JObject json)
        {
            try
            {
                if (json["traits"] is JObject traitsMap)
                {
                    List<string> traitKeys = traitsMap.Properties().Select(p => p.Name).ToList();
                    SyncTraitData(traitsMap);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[UnitEntity] Failed to parse StateData for Entity {Id}: {ex.Message}"
                );
            }
        }

        private void SyncTraitData(JObject traitsMap)
        {
            foreach (var trait in traitsMap)
            {
                string traitKey = trait.Key;
                Debug.Log($"[UnitEntity] Syncing Trait '{traitKey}' for Entity {Id}.");
            }
        }

        public override void OnSpawned()
        {
            Debug.Log(
                $"[UnitEntity] {Id} ({Type}) successfully initialized with {Traits.Count} traits."
            );
        }
    }
}
