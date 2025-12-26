using CosmicScavengers.Core.Systems.Entities;
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
        public override void OnSpawned()
        {
            Debug.Log(
                $"[UnitEntity] {Id} ({Type}) successfully initialized with {Traits.Count} traits."
            );
        }
    }
}
