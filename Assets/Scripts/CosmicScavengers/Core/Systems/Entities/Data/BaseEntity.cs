using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base.Traits.Data;
using CosmicScavengers.Core.Systems.Entities.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Data.Entities
{
    /// <summary>
    /// The "Boilerplate Remover".
    /// Most of your game entities should inherit from this.
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour, IEntity
    {
        [SerializeField]
        [Tooltip("The type identifier for this entity.")]
        private string type;
        public string Type
        {
            get => type;
            set => type = value;
        }

        public long Id { get; set; }

        [SerializeField]
        [Tooltip("List of traits attached to this entity.")]
        private List<BaseTrait> traits = new();
        public List<BaseTrait> Traits
        {
            get => traits;
            set => traits = value;
        }

        public GameObject TraitsContainer;

        public bool IsStatic { get; set; }

        public virtual void OnSpawned() { }

        public virtual void OnRemoved() { }
    }
}
