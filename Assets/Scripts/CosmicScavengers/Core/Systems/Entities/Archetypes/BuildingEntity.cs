using CosmicScavengers.Core.Systems.Entities;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Systems.Entities.Archetypes
{
    /// <summary>
    /// A generalized class for any static structure (Stations, Turrets, Modules).
    /// Handles common building logic like health displays or power states.
    /// </summary>
    public class BuildingEntity : EntityBase
    {
        [Header("Building Settings")]
        [SerializeField]
        private MeshRenderer[] statusLights;

        [SerializeField]
        private Color activeColor = Color.green;

        [SerializeField]
        private Color inactiveColor = Color.red;

        public override void UpdateState(object data)
        {
            if (data is PlayerEntityData buildingData)
            {
                // Generalized logic: All buildings might have an 'IsActive' flag in the proto
                // UpdateStatusLights(buildingData.IsActive);

                // You can still add specific logic based on a "Type" field in the data
                // if (buildingData.Type == "Turret") { ... }
            }
        }

        private void UpdateStatusLights(bool isActive)
        {
            foreach (var light in statusLights)
            {
                light.material.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
