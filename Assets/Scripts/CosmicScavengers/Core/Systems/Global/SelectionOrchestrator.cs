using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Traits.Archetypes;
using CosmicScavengers.Networking.Event.Channels.Data;
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
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to raise when entity sync data is received.")]
        private EntitySyncChannel entitySyncChannel;

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
            if (entitySyncChannel == null)
            {
                Debug.LogError("[SelectionOrchestrator] EntitySyncChannel reference is missing!");
            }
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
                    HandleSelectionClick();
                }
            }
        }

        private void HandleSelectionClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            int combinedMask = entityLayer | terrainLayer;

            if (!Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, combinedMask))
            {
                DeselectAll();
                return;
            }

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
            if (currentSelection.Count == 0)
            {
                return;
            }

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

                // TODO - Optimize: Create a reusable MoveRequest object/struct
                object moveRequest = new
                {
                    EntityId = entity.Id,
                    From = entity.transform.position,
                    To = targetPoint,
                };

                entitySyncChannel.Raise(moveRequest);

                Debug.Log(
                    $"[SelectionOrchestrator] Moving entity {entity.Id} from: {entity.transform.position} to: {targetPoint}."
                );
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
