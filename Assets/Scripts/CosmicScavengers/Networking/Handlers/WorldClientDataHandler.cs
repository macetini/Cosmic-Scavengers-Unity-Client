using System;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers
{
    public class WorldClientDataHandler
    {
        public static void Handle(byte[] incomingBytes)
        {
            try
            {
                WorldData currentWorld = WorldData.Parser.ParseFrom(incomingBytes);
                Debug.Log($"Loaded world: {currentWorld.WorldName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialization failed: {e.Message}");
            }
        }
    }
}
