using System;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers.Binary
{
    public class WorldClientDataHandler : MonoBehaviour, IBinaryCommandHandler
    {
        [Tooltip("Set to false to disable this handler.")]
        public bool Active = true;

        //[Tooltip("Event channel triggered when world data is received.")]
        //public EventChannel<WorldData> Channel;

        public NetworkBinaryCommand Command => NetworkBinaryCommand.REQUEST_WORLD_STATE_S;

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
                //Channel.Raise(currentWorld);
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
