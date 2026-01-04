using System;
using CosmicScavengers.Networking.Commands.Binary;
using CosmicScavengers.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Protobuf.WorldData;
using CosmicScavengers.Networking.Responses.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Derived
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
