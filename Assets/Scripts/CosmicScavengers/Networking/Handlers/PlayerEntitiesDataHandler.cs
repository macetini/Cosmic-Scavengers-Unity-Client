using System;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers
{
    public class PlayerEntitiesDataHandler
    {
        public static void Handle(byte[] incomingBytes)
        {
            try
            {
                var playerEntitiesData = PlayerEntityListData.Parser.ParseFrom(incomingBytes);
                Debug.Log($"Received {playerEntitiesData.Entities.Count} player entities.");

                foreach (PlayerEntityData entity in playerEntitiesData.Entities)
                {
                    ProcessEntity(entity);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialization failed: {e.Message}");
            }
        }

        private static void ProcessEntity(PlayerEntityData entity)
        {
            Debug.Log(
                $"Entity ID: {entity.Id}, Type: {entity.EntityType}, Pos: ({entity.PosX}, {entity.PosY})"
            );
        }
    }
}
