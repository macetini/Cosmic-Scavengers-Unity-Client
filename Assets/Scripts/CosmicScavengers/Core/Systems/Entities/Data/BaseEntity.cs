using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base.Traits.Data;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Traits.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Data.Entities
{
    /// <summary>
    /// The "Boilerplate Remover".
    /// Most of your game entities should inherit from this.
    /// </summary>
    public abstract class BaseEntity : MonoBehaviour, IEntity
    {
        public long Id { get; set; }

        public bool IsStatic { get; set; }

        [SerializeField]
        [Tooltip("The type identifier for this entity.")]
        private string type;
        public string Type
        {
            get => type;
            set => type = value;
        }

        [SerializeField]
        [Tooltip("List of traits attached to this entity.")]
        private List<BaseTrait> traits = new();
        public List<BaseTrait> Traits
        {
            get => GetAllTraits();
            set
            {
                traits = value;
                RebuildTraitCache();
            }
        }

        public GameObject TraitsContainer;
        private readonly Dictionary<Type, ITrait> traitCache = new();

        public void RebuildTraitCache()
        {
            traitCache.Clear();

            if (traits == null)
            {
                Debug.LogWarning($"[BaseEntity] Entity {Id} has no traits to cache.");
                return;
            }

            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i] == null)
                {
                    Debug.LogWarning(
                        $"[BaseEntity] Entity {Id} has a null trait at index {i}, skipping."
                    );
                    continue;
                }

                Type traitType = traits[i].GetType();
                if (!traitCache.ContainsKey(traitType))
                {
                    traitCache.Add(traitType, traits[i]);
                }
                else
                {
                    Debug.LogWarning(
                        $"[BaseEntity] Entity {Id} has duplicate trait of type {traitType.Name}, skipping."
                    );
                }
            }
        }

        public List<BaseTrait> GetAllTraits()
        {
            return new List<BaseTrait>(traits);
        }

        public T GetTrait<T>()
            where T : class, ITrait
        {
            Type targetType = typeof(T);
            if (traitCache.TryGetValue(targetType, out var cachedTrait))
            {
                return (T)cachedTrait;
            }

            // Fallback for traits added outside the standard setter
            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i] is T trait)
                {
                    traitCache[targetType] = trait;
                    Debug.LogWarning(
                        $"[BaseEntity] Trait of type {targetType.Name} was not cached for entity {Id}, adding to cache."
                    );
                    return trait;
                }
            }

            return null;
        }

        public virtual void OnSpawned() { }

        public virtual void OnRemoved()
        {
            traitCache.Clear();
            traits.Clear();
            traits = null;
        }
    }
}
