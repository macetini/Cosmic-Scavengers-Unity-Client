using System;
using CosmicScavengers.Networking.Protobuf.PlayerEntities;
using UnityEngine;

namespace CosmicScavengers.Networking.Handlers
{
    public class PlayerEntitiesDataHandler
    {
        public static void Handle(byte[] incomingBytes, int count)
        {
            try
            {
                Debug.Log($"Deserializing {count} player entities from byte array.");
                int messageLength = incomingBytes.Length / count;
                Debug.Log($"Message Length: {messageLength}, Count: {count}");
                int offset = 0;
                for (int i = 0; i < count; i++)
                {
                    PlayerEntityData playerEntityData = PlayerEntityData.Parser.ParseFrom(
                        incomingBytes,
                        offset,
                        messageLength
                    );

                    offset += messageLength;

                    Debug.Log(
                        $"Loaded player entity ID: {playerEntityData.Id}, Type: {playerEntityData.EntityType}"
                    );
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Deserialization failed: {e.Message}");
            }
        }
    }
}
