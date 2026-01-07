using System;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Gameplay.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Commands.Responses.Data.Binary;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Responses.Derived
{
    public class WorldDataResponse : BaseBinaryResponse
    {
        [Header("Channel Configuration")]
        [Tooltip("Event channel triggered when world data is received.")]
        [SerializeField]
        private WorldDataChannel Channel;
        public override NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_WORLD_STATE_S;

        void Awake()
        {
            if (Channel == null)
            {
                Debug.LogError("[WorldClientDataHandler] WorldDataChannel reference is missing!");
            }
        }

        public override void Handle(byte[] protobufData)
        {
            if (!Active)
            {
                Debug.LogWarning("[WorldClientDataHandler] Handler is inactive. Ignoring data.");
                return;
            }

            WorldData currentWorld;

            try
            {
                currentWorld = WorldData.Parser.ParseFrom(protobufData);
                Debug.Log(
                    $"[WorldClientDataHandler] Received data for World: {currentWorld.WorldName}"
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[WorldClientDataHandler] Failed to parse world data: {e.Message}");
                return;
            }

            Channel.Raise(currentWorld);
        }
    }
}
