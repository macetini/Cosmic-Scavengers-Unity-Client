using System.Collections.Generic;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Core.Assets
{
    /// <summary>
    /// Manages game assets such as entities, providing methods to spawn, update, and remove them.
    /// </summary>
    public class AssetManager : MonoBehaviour
    {
        // Assign in Inspector
        public GameObject spaceStationPrefab;

        private readonly Dictionary<long, GameObject> entities = new();

        public void HandleEntitiesUpdate(PlayerEntityData entityData)
        {
            SpawnEntity(
                entityData.Id,
                new Vector3(entityData.PosX, 0, entityData.PosY),
                Quaternion.identity
            );
            /*
            foreach (var entityData in updatedEntities)
            {
                Vector3 entityDataPosition = new(entityData.PosX, 0, entityData.PosY);
                Quaternion entityDataRotation = Quaternion.identity;
                SpawnEntity(entityData.Id, entityDataPosition, entityDataRotation);
            }
            */
        }

        private void SpawnEntity(long id, Vector3 pos, Quaternion rot)
        {
            if (entities.ContainsKey(id))
            {
                UpdateEntity(id, pos, rot);
                return;
            }

            GameObject obj = Instantiate(spaceStationPrefab, pos, rot);
            entities[id] = obj;

            Debug.Log($"[AssetManager] Spawned entity with ID: {id} at position: {pos}");
        }

        private void UpdateEntity(long id, Vector3 pos, Quaternion rot)
        {
            if (!entities.TryGetValue(id, out GameObject obj))
            {
                Debug.LogWarning(
                    $"[AssetManager] Attempted to update non-existent entity with ID: {id}"
                );
                return;
            }

            obj.transform.SetPositionAndRotation(pos, rot);
        }

        private void RemoveEntity(long id)
        {
            if (!entities.TryGetValue(id, out GameObject obj))
            {
                Debug.LogWarning(
                    $"[AssetManager] Attempted to remove non-existent entity with ID: {id}"
                );
                return;
            }

            Destroy(obj); // TODO - consider object pooling
            entities.Remove(id);
        }
    }
}
