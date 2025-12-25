using CosmicScavengers.Core.Systems.Entities;
using CosmicScavengers.Core.Systems.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class TraitBase : MonoBehaviour, IEntityTrait
    {
        // Reference to the Entity that owns this trait
        protected EntityBase Owner { get; private set; }

        // Shortcut to the Transform for performance
        protected Transform CachedTransform { get; private set; }
        public string Name
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public virtual void Initialize(EntityBase owner)
        {
            Owner = owner;
            CachedTransform = owner.transform;
            OnInitialize();
        }

        /// <summary>
        /// Optional override for subclasses to perform their own setup logic.
        /// </summary>
        protected virtual void OnInitialize() { }

        /// <summary>
        /// The update loop managed by EntityBase.
        /// </summary>
        public abstract void OnUpdate(float deltaTime);

        /// <summary>
        /// Helper to quickly find another trait on the same entity.
        /// </summary>
        protected T GetOtherTrait<T>()
            where T : class, IEntityTrait
        {
            if (Owner == null)
                return null;
            // Assuming EntityBase has a way to expose its list or we use GetComponent
            return Owner.GetComponentInChildren<T>();
        }
    }
}
