using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Base.Traits
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, IEntityTrait
    {
        // Reference to the Entity that owns this trait
        protected BaseEntity Owner { get; private set; }

        // Shortcut to the Transform for performance
        protected Transform CachedTransform { get; private set; }
        public string Name
        {
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        public virtual void Initialize(BaseEntity owner)
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
        /// The update loop managed by BaseEntity.
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
            // Assuming BaseEntity has a way to expose its list or we use GetComponent
            return Owner.GetComponentInChildren<T>();
        }
    }
}
