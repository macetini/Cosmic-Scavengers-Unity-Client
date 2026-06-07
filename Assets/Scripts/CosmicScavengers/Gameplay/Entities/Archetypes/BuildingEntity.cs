using CosmicScavengers.Core.Systems.Entities.Base;
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
            if (statusLights == null || statusLights.Length == 0)
            {
                return;
            }

            foreach (var light in statusLights)
            {
                if (light == null)
                {
                    continue;
                }

                light.material.color = isActive ? activeColor : inactiveColor;
            }
        }
    }
}
