using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Traits.Meta;
using CosmicScavengers.Networking.Protobuf.Entities;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Meta
{
    public interface IEntity
    {
        long Id { get; set; }
        string Type { get; set; }
        bool IsStatic { get; set; }
        Vector3 Position { get; set; }
        List<IEntityTrait> Traits { get; set; }

        void OnSpawned();
        void UpdateState(string data);
        void OnRemoved();
    }
}
