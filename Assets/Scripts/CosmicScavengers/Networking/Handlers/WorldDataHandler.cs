using System;
using CosmicScavengers.Core.Event;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers
{
    public class WorldClientDataHandler : MonoBehaviour, INetworkCommandHandler
    {
        [Tooltip("Set to false to disable this handler.")]
        public bool Active = true;

        [Tooltip("Event channel triggered when world data is received.")]
        public EventChannel<WorldData> Channel;

        public NetworkCommand CommandCode => NetworkCommand.REQUEST_WORLD_STATE_S;

        public void Handle(byte[] protobufData)
        {
            if (!Active)
            {
                Debug.LogWarning("[WorldClientDataHandler] Handler is inactive. Ignoring data.");
                return;
            }

            try
            {
                WorldData currentWorld = WorldData.Parser.ParseFrom(protobufData);
                Channel.Raise(currentWorld);
                Debug.Log(
                    $"[WorldClientDataHandler] Received data for World: {currentWorld.WorldName}"
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"[WorldClientDataHandler] Failed to parse world data: {e.Message}");
            }
        }
    }
}
