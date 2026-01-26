using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Networking.Protobuf.Traits;
using UnityEngine;

namespace CosmicScavengers.GamePlay.Entities.Traits.Archetypes
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

        private SelectableTraitProto data;

        private void Start()
        {
            selectionIndicator?.SetActive(false);
        }

        protected override void Initialize()
        {
            data = protoData as SelectableTraitProto;
            if (data == null)
            {
                Debug.LogError(
                    $"[SelectableTrait] Failed to cast ProtoData for entity {Owner?.Id}. Expected SelectableTraitProto."
                );
                return;
            }

            //Debug.Log($"[SelectableTrait] ShowHealthBar: [{data.SelectionRadius}].");
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
