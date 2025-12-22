using CosmicScavengers.Core.Systems.Entities.Meta;
using Unity.Collections;
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

        public long Id { get; set; }
        public string Type
        {
            get => type;
            set => type = value;
        }
        public bool IsStatic { get; set; }
        public Vector2 Position { get; set; }

        // Virtual methods allow child classes to choose what they implement
        public virtual void OnSpawned() { }

        // Abstract forces child classes to handle their own data parsing
        public abstract void UpdateState(object data);

        public virtual void OnRemoved() { }

        // Helper to quickly find the transform without repeated GetComponent calls
        protected Transform CachedTransform { get; private set; }

        protected virtual void Awake()
        {
            CachedTransform = transform;
        }
    }
}
