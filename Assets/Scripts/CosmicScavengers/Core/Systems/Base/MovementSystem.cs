using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Base;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using CosmicScavengers.GamePlay.Entities.Traits.Archetypes;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Movement
{
    public class MovementSystem : MonoBehaviour, IGameSystem
    {
        public string SystemName => "MovementSystem";
        private readonly List<MovableTrait> movableTraits = new();

        public void Register(ITrait trait)
        {
            if (trait is MovableTrait mover)
            {
                movableTraits.Add(mover);
            }
        }

        public void Unregister(ITrait trait)
        {
            if (trait is MovableTrait mover)
            {
                movableTraits.Remove(mover);
            }
        }

        public void OnTick(float deltaTime)
        {
            for (int i = 0; i < movableTraits.Count; i++)
            {
                var trait = movableTraits[i];
                if (!trait.PendingUpdate || !trait.IsEnabled)
                {
                    continue;
                }

                if (trait.Owner?.Transform != null)
                {
                    // Move the Entity, not the Trait
                    trait.Owner.Transform.position = Vector3.Lerp(
                        trait.Owner.Transform.position,
                        trait.TargetPosition,
                        deltaTime * 5f
                    );

                    // Optional: Handle Rotation on the Entity
                    if (trait.TargetPosition != trait.Owner.Transform.position)
                    {
                        // Rotation logic targeting trait.Owner.Transform...
                    }
                }

                trait.PendingUpdate = false;
            }
        }

        public void OnLateTick() { }
    }
}
