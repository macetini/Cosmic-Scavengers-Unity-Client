using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Traits.Meta;
using CosmicScavengers.Networking.Protobuf.Entities;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities
{
    /// <summary>
    /// The "Boilerplate Remover".
    /// Most of your game entities should inherit from this.
    /// </summary>
    public abstract class EntityBase : MonoBehaviour, IEntity
    {
        [SerializeField]
        private string type;
        public string Type
        {
            get => type;
            set => type = value;
        }

        public long Id { get; set; }
        public bool IsStatic { get; set; }
        public Vector3 Position { get; set; }

        [SerializeField]
        private List<IEntityTrait> traits = new();
        public List<IEntityTrait> Traits
        {
            get => traits;
            set => traits = value;
        }

        public virtual void OnSpawned() { }

        public abstract void UpdateState(string data);

        public virtual void OnRemoved() { }

        protected Transform CachedTransform { get; private set; }

        protected virtual void Awake()
        {
            CachedTransform = transform;
        }
    }
}
