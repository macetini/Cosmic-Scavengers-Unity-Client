using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Base;
using CosmicScavengers.GamePlay.Entities.Traits.Archetypes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CosmicScavengers.Core.Systems.Interaction
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
        private LayerMask terrainLayer;

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
                if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
                {
                    HandleSelection();
                }
            }
        }

        private void HandleSelection()
        {
            bool hitSomething = DidRayHitSomething(
                mainCamera.ScreenPointToRay(Input.mousePosition),
                out RaycastHit hit
            );

            if (!hitSomething)
            {
                DeselectAll();
                return;
            }

            HandleRaycastHit(hit);
        }

        private bool DidRayHitSomething(Ray ray, out RaycastHit hit)
        {
            Debug.Log(
                $"[SelectionOrchestrator] Performing raycast from camera: {mainCamera.name} at screen position: {Input.mousePosition}."
            );
            int combinedMask = entityLayer | terrainLayer;
            bool hitResult = Physics.Raycast(ray, out hit, maxRaycastDistance, combinedMask);

            Debug.Log(
                hitResult
                    ? $"[SelectionOrchestrator] Raycast hit: {hit.collider.gameObject.name} at position: {hit.point}."
                    : "[SelectionOrchestrator] Raycast did not hit any valid targets."
            );

            return hitResult;
        }

        private void HandleRaycastHit(RaycastHit hit)
        {
            Debug.Log(
                $"[SelectionOrchestrator] Raycast hit: {hit.collider.gameObject.name} at position: {hit.point}."
            );
            int hitLayer = 1 << hit.collider.gameObject.layer;

            if ((hitLayer & entityLayer) != 0)
            {
                HandleEntitySelection(hit.collider);
            }
            else if ((hitLayer & terrainLayer) != 0)
            {
                HandleTerrainClick(hit.point);
            }
        }

        private void HandleEntitySelection(Collider hitCollider)
        {
            Debug.Log(
                $"[SelectionOrchestrator] Raycast hit entity collider: {hitCollider.gameObject.name} at position: {hitCollider.transform.position}."
            );

            if (hitCollider.TryGetComponent<BaseEntity>(out var entity))
            {
                var selectable = entity.GetTrait<SelectableTrait>();
                if (selectable != null)
                {
                    // Cache input check to avoid multiple native calls
                    bool isMultiSelect =
                        Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                    ProcessSelection(selectable, isMultiSelect);
                    return;
                }
            }
            DeselectAll();
        }

        private void HandleTerrainClick(Vector3 targetPoint)
        {
            Debug.Log($"[SelectionOrchestrator] Raycast hit terrain at position: {targetPoint}.");
            if (currentSelection.Count == 0)
            {
                return;
            }

            Debug.Log(
                $"[SelectionOrchestrator] Issuing move command to {currentSelection.Count} selected entities."
            );
            foreach (var selectable in currentSelection)
            {
                BaseEntity entity = selectable.Owner as BaseEntity;
                if (entity == null)
                {
                    Debug.LogWarning(
                        $"[SelectionOrchestrator] Selected trait's owner is not a BaseEntity. Skipping entity ID {selectable.Owner?.Id}."
                    );
                    continue;
                }

                Debug.Log(
                    $"[SelectionOrchestrator] Initiating Move for entity {entity.Id} from: {entity.transform.position} to: {targetPoint}."
                );

                var movable = entity.GetTrait<MovableTrait>();
                if (movable == null)
                {
                    Debug.LogWarning(
                        $"[SelectionOrchestrator] Selected entity {entity.Id} has no MovableTrait. Terrain click ignored for this entity."
                    );
                    continue;
                }

                movable.IssueMoveOrder(targetPoint);
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

            Debug.Log("[SelectionOrchestrator] Deselecting all entities.");
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
