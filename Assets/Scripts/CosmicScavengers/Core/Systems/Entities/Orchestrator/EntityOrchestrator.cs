using System.Collections.Generic;
using System.Linq;
using CosmicScavengers.Core.Systems.Data.Entities;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entities.Registry;
using CosmicScavengers.Core.Systems.Traits;
using CosmicScavengers.Core.Systems.Traits.Registry;
using CosmicScavengers.Networking.Event.Channels;
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
        private PlayerEntitiesDataChannel Channel;

        [Header("Registry Configuration")]
        [Tooltip("Registry containing entity prefabs and metadata.")]
        [SerializeField]
        private EntityRegistry entityRegistry;

        [SerializeField]
        private TraitRegistry traitRegistry;

        [SerializeField]
        [Tooltip("Parent transform for spawned entities.")]
        private Transform entityParent;

        private readonly Dictionary<long, IEntity> activeEntities = new();

        void Awake()
        {
            if (Channel == null)
            {
                Debug.LogError("[EntityOrchestrator] PlayerEntitiesDataChannel reference is missing!");
            }
            if (entityRegistry == null)
            {
                Debug.LogError("[EntityOrchestrator] EntityRegistry reference is missing!");
            }
            if (traitRegistry == null)
            {
                Debug.LogError("[EntityOrchestrator] TraitRegistry reference is missing!");
            }
        }

        void OnEnable()
        {
            Channel.AddListener(OnPlayerEntitiesDataReceived);
        }

        void OnDisable()
        {
            Channel.RemoveListener(OnPlayerEntitiesDataReceived);
        }

        private void OnPlayerEntitiesDataReceived(EntitySyncResponse syncResponse)
        {
            Debug.Log(
                $"[EntityOrchestrator] Syncing {syncResponse.Entities.Count} player entities from network data."
            );

            foreach (PlayerEntityProto entityData in syncResponse.Entities)
            {
                SyncEntity(
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
        public void SyncEntity(
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
            GameObject prefab = entityRegistry.GetPrefab(typeKey);
            if (prefab == null)
            {
                Debug.LogError($"[EntityOrchestrator] Registry lookup failed for key: {typeKey}");
                return;
            }

            GameObject instance = Instantiate(prefab, position, rotation, entityParent);
            if (!instance.TryGetComponent<BaseEntity>(out var entity))
            {
                Debug.LogError(
                    $"[EntityOrchestrator] Prefab '{typeKey}' missing IEntity implementation!"
                );
                Destroy(instance);
                return;
            }

            entity.Id = id;
            activeEntities.Add(id, entity);

            if (!string.IsNullOrEmpty(stateData))
            {
                entity.Traits = GetEntityTraits(stateData);
            }
            entity.OnSpawned();
        }

        private List<TraitBase> GetEntityTraits(string stateData)
        {
            List<TraitBase> traits = new();
            try
            {
                JObject json = JObject.Parse(stateData);
                if (json["traits"] is JObject traitsMap)
                {
                    List<string> traitKeys = traitsMap.Properties().Select(p => p.Name).ToList();
                    foreach (string traitKey in traitKeys)
                    {
                        TraitBase traitPrefab = traitRegistry.GetPrefab(traitKey);
                        if (traitPrefab == null)
                        {
                            Debug.LogWarning(
                                $"[EntityOrchestrator] Trait lookup failed for key: {traitKey}"
                            );
                            continue;
                        }
                        traits.Add(traitPrefab);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    $"[EntityOrchestrator] Failed to parse StateData for traits: {ex.Message}"
                );
            }
            return traits;
        }

        private void UpdateEntityInstance(
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
                entity.Traits = GetEntityTraits(stateData);
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
                    Destroy(mb.gameObject);
            }
            activeEntities.Clear();
        }
    }
}
