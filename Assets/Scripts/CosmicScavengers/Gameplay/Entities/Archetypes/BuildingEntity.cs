using CosmicScavengers.Core.Systems.Data.Entities;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Entities.Archetypes
{
    /// <summary>
    /// A generalized class for any static structure (Stations, Turrets, Modules).
    /// Handles common building logic like health displays or power states.
    /// </summary>
    public class BuildingEntity : BaseEntity
    {
        [Header("Building Settings")]
        [SerializeField]
        private MeshRenderer[] statusLights;

        [SerializeField]
        private Color activeColor = Color.green;

        [SerializeField]
        private Color inactiveColor = Color.red;

        private void UpdateStatusLights(bool isActive)
        {
            foreach (var light in statusLights)
            {
                light.material.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
