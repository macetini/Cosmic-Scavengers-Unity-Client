using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Traits.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Base.Traits.Data
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, ITrait
    {
        protected IEntity Owner { get; private set; }

        [SerializeField]
        private string traitName;
        public string Name
        {
            get => string.IsNullOrEmpty(traitName) ? GetType().Name : traitName;
            set => traitName = value;
        }

        public virtual int UpdateFrequency => 1;

        public virtual void Initialize(IEntity owner)
        {
            Owner = owner;
            OnInitialize();
        }

        /// <summary>
        /// Optional override for subclasses to perform their own setup logic.
        /// </summary>
        protected virtual void OnInitialize() { }

        public bool OwnedBySameEntity(BaseTrait otherTrait)
        {
            return otherTrait != null
                && Owner != null
                && otherTrait.Owner != null
                && Owner.Id == otherTrait.Owner.Id;
        }

        /// <summary>
        /// The update loop managed by BaseEntity.
        /// </summary>
        public abstract void OnUpdate(float deltaTime);
    }
}
