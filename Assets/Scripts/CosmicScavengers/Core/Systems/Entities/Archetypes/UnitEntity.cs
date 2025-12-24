using CosmicScavengers.Core.Systems.Entities;
using CosmicScavengers.Networking.Protobuf.Entities;
using UnityEngine;

namespace CosmicScavengers.Systems.Entities.Archetypes
{
    /// <summary>
    /// A generalized class for any mobile unit (Ships, Drones).
    /// Handles common unit logic like movement and actions.
    /// </summary>
    public class UnitEntity : EntityBase
    {
        public override void UpdateState(object data)
        {
            if (data is PlayerEntityProto unitData)
            {
                // Generalized logic for units
                // e.g., transform.position = new Vector3(unitData.Position.X, unitData.Position.Y, unitData.Position.Z);
            }
        }
    }
}
