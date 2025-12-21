using System;
using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers.Binary
{
    public class PlayerEntitiesDataHandler : MonoBehaviour, IBinaryCommandHandler
    {
        public bool Active = true;

        //public EventChannel<PlayerEntityData> Channel;

        public NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_S;

        public void Handle(byte[] protobufData)
        {
            if (!Active)
            {
                Debug.LogWarning("[PlayerEntitiesDataHandler] Handler is inactive. Ignoring data.");
                return;
            }

            try
            {
                var playerEntitiesData = PlayerEntityListData.Parser.ParseFrom(protobufData);
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

        private void ProcessEntity(PlayerEntityData entity)
        {
            Debug.Log(
                $"Entity ID: {entity.Id}, Type: {entity.EntityType}, Pos: ({entity.PosX}, {entity.PosY})"
            );

            //Channel.Raise(entity);
        }
    }
}
