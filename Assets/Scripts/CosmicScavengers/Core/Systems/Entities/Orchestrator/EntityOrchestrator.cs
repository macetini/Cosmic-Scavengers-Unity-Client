using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Base;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entities.Registry;
using CosmicScavengers.Core.Systems.Entities.Traits.Registry;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using CosmicScavengers.Core.Systems.Traits.Processor;
using CosmicScavengers.Gameplay.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Protobuf.Entities;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Orchestrator
{
    /// <summary>
    /// Provides services for spawning, updating, and removing networked entities.
    /// Orchestrates the relationship between network data and Unity GameObjects.
    /// </summary>
    public class EntityOrchestrator : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [Tooltip("Channel to raise when player entities data is received.")]
        [SerializeField]
        private PlayerEntitiesDataChannel playerEntitiesDataChannel;

        [Header("Registry Configuration")]
        [Tooltip("Registry containing entity prefabs and metadata.")]
        [SerializeField]
        private EntityRegistry entityRegistry;

        [Tooltip("Registry containing trait prefabs.")]
        [SerializeField]
        private TraitRegistry traitRegistry;

        [Header("Orchestrator Configuration")]
        [Tooltip("Traits processor responsible for trait update and synchronization cycles.")]
        [SerializeField]
        private TraitsProcessor traitsProcessor;

        [Tooltip("Parent transform for spawned entities.")]
        [SerializeField]
        private Transform entityParent; // TODO. TRANSFER SOMEWHERE ELSE

        private readonly Dictionary<long, IEntity> activeEntities = new();
        private const string TRAITS_KEY = "traits";

        void Awake()
        {
            if (playerEntitiesDataChannel == null)
            {
                Debug.LogError(
                    "[EntityOrchestrator] PlayerEntitiesDataChannel reference is missing!"
                );
            }
            if (entityRegistry == null)
            {
                Debug.LogError("[EntityOrchestrator] EntityRegistry reference is missing!");
            }
            if (traitRegistry == null)
            {
                Debug.LogError("[EntityOrchestrator] TraitRegistry reference is missing!");
            }
            if (traitsProcessor == null)
            {
                Debug.LogError("[EntityOrchestrator] TraitsProcessor reference is missing!");
            }
            if (entityParent == null)
            {
                Debug.LogError("[EntityOrchestrator] EntityParent reference is missing!");
            }
        }

        void OnEnable()
        {
            playerEntitiesDataChannel.AddListener(OnPlayerEntitiesDataReceived);
        }

        void OnDisable()
        {
            playerEntitiesDataChannel.RemoveListener(OnPlayerEntitiesDataReceived);
        }

        private void OnPlayerEntitiesDataReceived(EntitySyncResponse syncResponse)
        {
            Debug.Log(
                $"[EntityOrchestrator] Syncing {syncResponse.Entities.Count} player entities from network data."
            );

            foreach (PlayerEntityProto entityData in syncResponse.Entities)
            {
                SyncEntity( // TODO - Less arguments
                    entityData.Id,
                    entityData.BlueprintId,
                    new Vector3(entityData.PosX, entityData.PosY, entityData.PosZ),
                    Quaternion.Euler(0, entityData.Rotation, 0),
                    entityData.StateData
                );
            }
        }

        /// <summary>
        /// Synchronizes an entity based on network data.
        /// Spawns the entity if it doesn't exist, otherwise updates it.
        /// </summary>
        public void SyncEntity( // TODO - Less arguments
            long id,
            string typeKey,
            Vector3 position,
            Quaternion rotation,
            string stateData
        )
        {
            if (activeEntities.TryGetValue(id, out var entity))
            {
                UpdateEntityInstance(entity, position, rotation, stateData);
            }
            else
            {
                SpawnEntityInstance(id, typeKey, position, rotation, stateData);
            }
        }

        private void SpawnEntityInstance(
            long id,
            string typeKey,
            Vector3 position,
            Quaternion rotation,
            string stateData
        )
        {
            BaseEntity entityPrefab = entityRegistry.GetPrefab(typeKey);
            if (entityPrefab == null)
            {
                Debug.LogError($"[EntityOrchestrator] Registry lookup failed for key: {typeKey}");
                return;
            }

            BaseEntity spawnedEntity = Instantiate(entityPrefab, position, rotation, entityParent);
            spawnedEntity.LinkTraitsProcessor(traitsProcessor);

            spawnedEntity.Id = id;
            activeEntities.Add(id, spawnedEntity);

            if (!string.IsNullOrEmpty(stateData))
            {
                SetEntityTraits(stateData, spawnedEntity);
            }
            spawnedEntity.OnSpawned();
        }

        private void SetEntityTraits(string stateData, BaseEntity entity)
        {
            try
            {
                JObject json = JObject.Parse(stateData);
                if (json[TRAITS_KEY] is JObject traitsMap)
                {
                    List<ITrait> traits = new();
                    foreach (var property in traitsMap.Properties())
                    {
                        if (property.Value is not JObject traitConfig)
                        {
                            Debug.LogWarning(
                                $"[EntityOrchestrator] Trait config for '{property.Name}' is not a JObject. Skipping."
                            );
                            continue;
                        }
                        string traitKey = property.Name;
                        BaseTrait traitPrefab = traitRegistry.GetPrefab(traitKey);
                        if (traitPrefab == null)
                        {
                            Debug.LogWarning(
                                $"[EntityOrchestrator] Trait prefab not found: {traitKey}"
                            );
                            continue;
                        }
                        BaseTrait traitInstance = Instantiate(
                            traitPrefab,
                            entity.TraitsContainer.transform
                        );
                        traitInstance.Initialize(entity, traitConfig);
                        traits.Add(traitInstance);
                    }
                    entity.Traits = traits;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EntityOrchestrator] Failed to parse StateData: {ex.Message}");
            }
        }

        private void UpdateEntityInstance( // TODO - Finish this.
            IEntity entity,
            Vector3 position,
            Quaternion rotation,
            string stateData
        )
        {
            if (entity is MonoBehaviour mb)
            {
                mb.transform.SetPositionAndRotation(position, rotation);
            }

            if (!string.IsNullOrEmpty(stateData))
            {
                //entity.Traits = GetEntityTraits(stateData);
            }
        }

        /// <summary>
        /// Removes an entity from the simulation.
        /// </summary>
        public void RemoveEntity(long id)
        {
            if (activeEntities.TryGetValue(id, out var entity))
            {
                entity.OnRemoved();
                if (entity is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
                activeEntities.Remove(id);
            }
        }

        /// <summary>
        /// Clears all managed entities (useful for scene transitions or logouts).
        /// </summary>
        public void ClearAll()
        {
            foreach (var entity in activeEntities.Values)
            {
                entity.OnRemoved();
                if (entity is MonoBehaviour mb)
                {
                    Destroy(mb.gameObject);
                }
            }
            activeEntities.Clear();
        }
    }
}
