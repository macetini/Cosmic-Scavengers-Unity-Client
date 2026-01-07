using CosmicScavengers.Core.Systems.Base.Traits.Data;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds selection capability to a GameObject.
    /// Following the Trait pattern allows entities to be composed of different
    /// functional blocks (Selection, Movement, Combat) independently.
    /// </summary>
    public class MovableTrait : BaseTrait
    {
        private void Start() { }

        public override void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
