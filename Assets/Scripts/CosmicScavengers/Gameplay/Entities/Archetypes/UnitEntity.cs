using CosmicScavengers.Core.Systems.Entities.Base;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Entities.Archetypes
{
    /// <summary>
    /// A generalized class for any mobile unit (Ships, Drones).
    /// Handles common unit logic like movement and actions.
    /// </summary>
    public class UnitEntity : BaseEntity
    {
        /// <summary>
        /// This overrides the abstract method in BaseEntity to handle
        /// the specific Protobuf message type.
        /// </summary>
        public override void OnSpawned()
        {
            base.OnSpawned();
            //Debug.Log($"[UnitEntity] {Id} ({Type}) successfully initialized with {Traits.Count} traits.");
        }
    }
}
