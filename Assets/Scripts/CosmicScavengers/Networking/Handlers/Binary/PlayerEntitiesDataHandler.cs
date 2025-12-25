using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Handlers.Binary;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.Entities;
using Google.Protobuf.Collections;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers.Binary
{
    public class PlayerEntitiesDataHandler : MonoBehaviour, IBinaryCommandHandler
    {
        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Set to false to disable this handler.")]
        private bool Active = true;

        [Header("Channel Configuration")]
        [Tooltip("Channel to raise when player entities data is received.")]
        [SerializeField]
        private PlayerEntitiesDataChannel Channel;

        public NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_S;

        void Start()
        {
            if (Channel == null)
            {
                Debug.LogError(
                    "[PlayerEntitiesDataHandler] PlayerEntitiesDataChannel reference is missing!"
                );
            }
        }

        public void Handle(byte[] protobufData)
        {
            if (!Active)
            {
                Debug.LogWarning("[PlayerEntitiesDataHandler] Handler is inactive. Ignoring data.");
                return;
            }

            EntitySyncResponse entitySyncResponse;

            try
            {
                entitySyncResponse = EntitySyncResponse.Parser.ParseFrom(protobufData);

                RepeatedField<PlayerEntityProto> entities = entitySyncResponse.Entities;

                Debug.Log(
                    $"[PlayerEntitiesDataHandler] Received {entities.Count} player entities."
                );
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[PlayerEntitiesDataHandler] Error while parsing PlayerEntityListData: {e.Message}"
                );
                return;
            }

            Channel.Raise(entitySyncResponse);
        }
    }
}
