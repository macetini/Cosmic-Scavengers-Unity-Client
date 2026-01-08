using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Traits.Data.Meta;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Base.Traits.Data
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, ITrait
    {
        [SerializeField]
        [Tooltip("The display name of this trait. If empty, the class name will be used.")]
        private string traitName;
        public string Name
        {
            get => string.IsNullOrEmpty(traitName) ? GetType().Name : traitName;
            set => traitName = value;
        }
        public IEntity Owner { get; private set; }
        protected JObject Data;
        public virtual int UpdateFrequency => 1;

        public virtual void Initialize(IEntity owner, JObject data = null)
        {
            Owner = owner;
            Data = data;
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
