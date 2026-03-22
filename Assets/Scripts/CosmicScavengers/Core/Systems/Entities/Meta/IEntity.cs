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
        Transform TraitsContainer { get; }
        Transform Transform { get; }

        void OnSpawned();
        void OnRemoved();
    }
}
