using System;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Handlers.Binary;
using CosmicScavengers.Networking.Event.Channels;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers.Binary
{
    public class WorldClientDataHandler : MonoBehaviour, IBinaryCommandHandler
    {
        [Header("Configuration")]
        [SerializeField]
        [Tooltip("Set to false to disable this handler.")]
        private bool Active = true;

        [Header("Channel Configuration")]
        [Tooltip("Event channel triggered when world data is received.")]
        [SerializeField]
        private WorldDataChannel Channel;

        public NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_WORLD_STATE_S;

        void Start()
        {
            if (Channel == null)
            {
                Debug.LogError("[WorldClientDataHandler] WorldDataChannel reference is missing!");
            }
        }

        public void Handle(byte[] protobufData)
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
