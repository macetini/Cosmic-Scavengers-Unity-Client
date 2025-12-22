using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entities.Registry;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities
{
    /// <summary>
    /// Provides services for spawning, updating, and removing networked entities.
    /// Orchestrates the relationship between network data and Unity GameObjects.
    /// </summary>
    public class EntityService : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to raise when player entities data is received.")]
        private PlayerEntitiesDataChannel Channel;

        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Registry containing entity prefabs and metadata.")]
        private EntityRegistry registry;

        [SerializeField]
        [Tooltip("Parent transform for spawned entities.")]
        private Transform entityParent;

        private readonly Dictionary<long, IEntity> activeEntities = new();

        void Awake()
        {
            if (registry == null)
            {
                Debug.LogError("[EntityService] EntityRegistry reference is missing!");
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

        private void OnPlayerEntitiesDataReceived(PlayerEntityListData data)
        {
            Debug.Log(
                $"[EntityService] Syncing {data.Entities.Count} player entities from network data."
            );

            foreach (var entityData in data.Entities)
            {
                SyncEntity(
                    entityData.Id,
                    entityData.EntityType,
                    new Vector3(entityData.PosX, 0, entityData.PosY),
                    Quaternion.identity
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
            object payload = null
        )
        {
            if (activeEntities.TryGetValue(id, out var entity))
            {
                UpdateEntityInstance(entity, position, rotation, payload);
            }
            else
            {
                SpawnEntityInstance(id, typeKey, position, rotation, payload);
            }
        }

        private void SpawnEntityInstance(
            long id,
            string typeKey,
            Vector3 position,
            Quaternion rotation,
            object payload
        )
        {
            GameObject prefab = registry.GetPrefab(typeKey);
            if (prefab == null)
            {
                Debug.LogError($"[EntityService] Registry lookup failed for key: {typeKey}");
                return;
            }

            GameObject instance = Instantiate(prefab, position, rotation, entityParent);

            if (!instance.TryGetComponent<IEntity>(out var entity))
            {
                Debug.LogError(
                    $"[EntityService] Prefab '{typeKey}' missing IEntity implementation!"
                );
                Destroy(instance);
                return;
            }

            entity.Id = id;
            activeEntities.Add(id, entity);

            entity.OnSpawned();
            if (payload != null)
                entity.UpdateState(payload);
        }

        private void UpdateEntityInstance(
            IEntity entity,
            Vector3 position,
            Quaternion rotation,
            object payload
        )
        {
            if (entity is MonoBehaviour mb)
            {
                mb.transform.SetPositionAndRotation(position, rotation);
            }

            if (payload != null)
            {
                entity.UpdateState(payload);
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
