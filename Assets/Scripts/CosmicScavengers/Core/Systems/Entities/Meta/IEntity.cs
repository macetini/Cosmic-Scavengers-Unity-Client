using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Meta
{
    public interface IEntity
    {
        long Id { get; set; }
        bool IsStatic { get; set; }
        string Type { get; set; }
        List<ITrait> Traits { get; set; }

        Transform Transform { get; }

        /// <summary>
        /// The standardized doorway for Traits to signal they need a network sync.
        /// The implementation (BaseEntity) handles the routing.
        /// </summary>
        void RequestTraitSync(ITrait trait);
        void OnSpawned();
        void OnRemoved();
    }
}
