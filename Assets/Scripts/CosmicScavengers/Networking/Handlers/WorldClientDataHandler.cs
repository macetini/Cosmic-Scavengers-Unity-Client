using System;
using Cosmic.Scavengers.Generated;
using UnityEngine;

public class WorldClientDataHandler
{
    public static void Handle(byte[] incomingBytes)
    {
        try
        {
            WorldData currentWorld = WorldData.Parser.ParseFrom(incomingBytes, sizeof(short), incomingBytes.Length - sizeof(short));
            Debug.Log($"Loaded world: {currentWorld.WorldName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Deserialization failed: {e.Message}");
        }
    }
}
