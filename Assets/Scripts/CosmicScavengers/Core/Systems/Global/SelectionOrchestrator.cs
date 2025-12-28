using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Traits.Archetypes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CosmicScavengers.Core.Systems.Global
{
    /// <summary>
    /// The high-performance "Brain" for selection.
    /// Performs a single raycast on click and manages the selection state of all entities.
    /// </summary>
    public class SelectionOrchestrator : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Layer mask for entities that can be selected.")]
        private LayerMask entityLayer;

        [SerializeField]
        [Tooltip("Camera used for raycasting. If null, defaults to Main Camera.")]
        private Camera mainCamera;

        [SerializeField]
        [Tooltip("Maximum distance for raycasting.")]
        private float maxRaycastDistance = 500f;

        private readonly List<SelectableTrait> currentSelection = new();

        public List<SelectableTrait> GetSelection() => currentSelection;

        void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                Debug.Log(
                    "[SelectionOrchestrator] Main Camera not assigned, defaulting to Main Camera."
                );
            }
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                HandleSelectionClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                DeselectAll();
            }
        }

        private void HandleSelectionClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, entityLayer))
            {
                if (!hit.collider.TryGetComponent<BaseEntity>(out var entity))
                {
                    DeselectAll();
                    return;
                }

                SelectableTrait selectableTrait = entity.GetTrait<SelectableTrait>();
                if (selectableTrait != null)
                {
                    bool isMultiSelect =
                        Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    ProcessSelection(selectableTrait, isMultiSelect);
                }
                else
                {
                    DeselectAll();
                }
            }
        }

        private void ProcessSelection(SelectableTrait trait, bool isMultiSelect)
        {
            if (!isMultiSelect)
            {
                if (trait.IsSelected)
                {
                    if (currentSelection.Count > 1)
                    {
                        DeselectAllExcept(trait);
                    }
                    else
                    {
                        DeselectAll();
                    }
                }
                else
                {
                    trait.Select();
                    currentSelection.Add(trait);
                }
            }
            else
            {
                if (trait.IsSelected)
                {
                    trait.Deselect();
                    currentSelection.Remove(trait);
                }
                else
                {
                    trait.Select();
                    currentSelection.Add(trait);
                }
            }
        }

        private void DeselectAll()
        {
            if (currentSelection.Count == 0)
            {
                return;
            }
            for (int i = 0; i < currentSelection.Count; i++)
            {
                if (currentSelection[i] != null)
                {
                    currentSelection[i].Deselect();
                }
            }
            currentSelection.Clear();
        }

        private void DeselectAllExcept(SelectableTrait exceptionTrait)
        {
            if (currentSelection.Count == 0)
            {
                return;
            }
            for (int i = currentSelection.Count - 1; i >= 0; i--)
            {
                var trait = currentSelection[i];
                if (trait != null && trait != exceptionTrait)
                {
                    trait.Deselect();
                    currentSelection.RemoveAt(i);
                }
            }
        }
    }
}
