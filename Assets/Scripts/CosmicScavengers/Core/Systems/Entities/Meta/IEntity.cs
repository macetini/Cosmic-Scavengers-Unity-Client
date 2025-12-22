using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Meta
{
    public interface IEntity
    {
        long Id { get; set; }
        string Type { get; set; }
        bool IsStatic { get; set; }
        Vector2 Position { get; set; }

        void OnSpawned();
        void UpdateState(object data);
        void OnRemoved();
    }
}
