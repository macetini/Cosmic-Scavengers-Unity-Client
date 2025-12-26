using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Traits.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Base.Traits.Data
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, ITrait
    {
        protected BaseEntity Owner { get; private set; }

        [SerializeField]
        private string traitName;
        public string Name
        {
            get => string.IsNullOrEmpty(traitName) ? GetType().Name : traitName;
            set => traitName = value;
        }

        public virtual int UpdateFrequency => 1;

        public virtual void Initialize(BaseEntity owner)
        {
            Owner = owner;
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
            where T : class, ITrait
        {
            if (Owner == null)
                return null;
            // Assuming BaseEntity has a way to expose its list or we use GetComponent
            return Owner.GetComponentInChildren<T>();
        }
    }
}
