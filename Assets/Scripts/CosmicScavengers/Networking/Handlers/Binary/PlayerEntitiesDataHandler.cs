using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Handlers.Binary;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers.Binary
{
    public class PlayerEntitiesDataHandler : MonoBehaviour, IBinaryCommandHandler
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to raise when player entities data is received.")]
        private PlayerEntitiesDataChannel Channel;

        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Set to false to disable this handler.")]
        private bool Active = true;

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
                Debug.Log(
                    $"[PlayerEntitiesDataHandler] Received {playerEntitiesData.Entities.Count} player entities."
                );

                Channel.Raise(playerEntitiesData);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    $"[PlayerEntitiesDataHandler] Error parsing PlayerEntityListData: {e.Message}"
                );
            }
        }
    }
}
