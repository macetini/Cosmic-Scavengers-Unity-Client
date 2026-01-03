using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Responses.Data;
using CosmicScavengers.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Protobuf.Entities;
using Google.Protobuf.Collections;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Derived
{
    public class PlayerEntitiesResponse : BaseBinaryResponse
    {
        [Header("Channel Configuration")]
        [Tooltip("Channel to raise when player entities data is received.")]
        [SerializeField]
        private PlayerEntitiesDataChannel Channel;

        public override NetworkBinaryCommand Command =>
            NetworkBinaryCommand.REQUEST_PLAYER_ENTITIES_S;

        void Awake()
        {
            if (Channel == null)
            {
                Debug.LogError(
                    "[PlayerEntitiesDataHandler] PlayerEntitiesDataChannel reference is missing!"
                );
            }
        }

        public override void Handle(byte[] protobufData)
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
