using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using CosmicScavengers.GamePlay.Entities.Traits.Archetypes;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Movement
{
    public class SelectionSystem : MonoBehaviour, IGameSystem
    {
        public string SystemName => "SelectionSystem";
        private readonly List<SelectableTrait> selectionTraits = new();

        public void Register(ITrait trait)
        {
            if (trait is SelectableTrait mover)
            {
                selectionTraits.Add(mover);
            }
        }

        public void Unregister(ITrait trait)
        {
            if (trait is SelectableTrait mover)
            {
                selectionTraits.Remove(mover);
            }
        }

        public void OnTick(float deltaTime)
        {
            foreach (var trait in selectionTraits)
            {
                if (!trait.PendingUpdate) // || !mover.IsEnabled)
                    continue;

                trait.PendingUpdate = false;
            }
        }

        public void OnLateTick() { }
    }
}
