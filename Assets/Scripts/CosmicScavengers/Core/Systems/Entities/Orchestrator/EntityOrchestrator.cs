using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Base;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entities.Registry;
using CosmicScavengers.Core.Systems.Entities.Trait.Factory;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using CosmicScavengers.Core.Systems.Traits.Processor;
using CosmicScavengers.Core.Systems.Utils.Scale4f;
using CosmicScavengers.Gameplay.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Protobuf.Entities;
using CosmicScavengers.Networking.Protobuf.Traits;
using Google.Protobuf;
using Google.Protobuf.Collections;
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

        [Header("Traits Configuration")]
        [SerializeField]
        [Tooltip("Trait factory responsible for instantiating traits onto entities.")]
        private TraitFactory traitFactory;

        [Tooltip("Traits processor responsible for trait update and synchronization cycles.")]
        [SerializeField]
        private TraitsProcessor traitsProcessor;

        [Header("Orchestrator Configuration")]
        [Tooltip("Parent transform for spawned entities.")]
        [SerializeField]
        private Transform entityParent; // TODO. TRANSFER SOMEWHERE ELSE

        private readonly Dictionary<long, IEntity> activeEntities = new();

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
            if (traitFactory == null)
            {
                Debug.LogError("[EntityOrchestrator] TraitFactory reference is missing!");
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
                SyncEntity(entityData);
            }
        }

        /// <summary>
        /// Synchronizes an entity based on network data.
        /// Spawns the entity if it doesn't exist, otherwise updates it.
        /// </summary>
        public void SyncEntity(PlayerEntityProto entityData)
        {
            Vector3 worldPos = new(
                DeterministicUtils.FromScaled(entityData.PosX),
                DeterministicUtils.FromScaled(entityData.PosY),
                DeterministicUtils.FromScaled(entityData.PosZ)
            );
            Quaternion rotation = Quaternion.Euler(0, entityData.Rotation, 0);

            if (activeEntities.TryGetValue(entityData.Id, out var entity))
            {
                UpdateEntityInstance(entity, worldPos, rotation, entityData.Traits);
            }
            else
            {
                SpawnEntityInstance(entityData, worldPos, rotation);
            }
        }

        private void SpawnEntityInstance(
            PlayerEntityProto entityData,
            Vector3 position,
            Quaternion rotation
        )
        {
            //TODO - Switch to IEntity from BaseEntity
            BaseEntity entityPrefab = entityRegistry.GetPrefab(entityData.BlueprintId);
            if (entityPrefab == null)
            {
                Debug.LogError(
                    $"[EntityOrchestrator] Registry lookup failed for key: {entityData.BlueprintId}"
                );
                return;
            }
            //TODO - Switch to IEntity from BaseEntity
            BaseEntity spawnedEntity = Instantiate(entityPrefab, position, rotation, entityParent);

            spawnedEntity.Id = entityData.Id;
            activeEntities.Add(spawnedEntity.Id, spawnedEntity);

            ProcessEntityTraits(entityData.Traits, spawnedEntity);
            spawnedEntity.OnSpawned();
        }

        private void UpdateEntityInstance( // TODO - Finish this.
            IEntity entity,
            Vector3 position,
            Quaternion rotation,
            RepeatedField<TraitInstanceProto> traitProtos
        )
        {
            if (entity is MonoBehaviour mb)
            {
                mb.transform.SetPositionAndRotation(position, rotation);
            }
        }

        private void ProcessEntityTraits(
            RepeatedField<TraitInstanceProto> traitProtos,
            IEntity entity
        )
        {
            if (traitProtos == null || traitProtos.Count == 0)
            {
                Debug.LogWarning($"[EntityOrchestrator] Entity {entity.Id} has no traits.");
                return;
            }

            List<IMessage> traitsProtoData = traitFactory.ParseTraitProtoData(traitProtos);
            List<ITrait> traits = traitFactory.CreateAndAttachTraits(
                traitsProtoData,
                entity.TraitsContainer
            );
            entity.Traits = traits;
            traitsProcessor.Register(traits);
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
