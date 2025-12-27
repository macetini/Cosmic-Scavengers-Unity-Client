using CosmicScavengers.Core.Systems.Base.Traits.Data;
using CosmicScavengers.Core.Systems.Entities.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Traits.Archetypes
{
    /// <summary>
    /// A modular "Trait" that adds selection capability to a GameObject.
    /// Following the Trait pattern allows entities to be composed of different
    /// functional blocks (Selection, Movement, Combat) independently.
    /// </summary>
    public class SelectableTrait : BaseTrait
    {
        [Header("Visual Feedback")]
        [SerializeField]
        private GameObject selectionIndicator;
        public bool IsSelected { get; private set; }

        private void Start()
        {
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }
        }

        public void ToggleSelection(bool state)
        {
            if (state)
            {
                Select();
            }
            else
            {
                Deselect();
            }
        }

        public void Select()
        {
            if (IsSelected)
            {
                return;
            }

            IsSelected = true;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(true);
            }

            Debug.Log($"[SelectableTrait] Entity {Owner.Id} selected.");
        }

        public void Deselect()
        {
            if (!IsSelected)
            {
                return;
            }

            IsSelected = false;
            if (selectionIndicator != null)
            {
                selectionIndicator.SetActive(false);
            }

            Debug.Log($"[SelectableTrait] Entity {Owner.Id} deselected.");
        }

        public override void OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
