using System;
using System.Collections.Generic;
using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;
using Type = System.Type;

namespace CosmicScavengers.Core.Networking.Mappers
{
    public class TraitProtobufMapper
    {
        private readonly Dictionary<string, Type> typeCache = new();
        private const string PROTO_NAMESPACE = "CosmicScavengers.Networking.Protobuf.Traits";

        public TraitProtobufMapper()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace == PROTO_NAMESPACE && typeof(IMessage).IsAssignableFrom(type))
                {
                    string traitId = type.Name.Replace("TraitProto", "").ToLower();
                    typeCache[traitId] = type;
                }
            }
        }

        public IMessage MapFromProto(string traitId, Any data)
        {
            if (!typeCache.TryGetValue(traitId.ToLower(), out Type targetType))
            {
                Debug.LogWarning($"[TraitProtobufMapper] No C# class found for TraitId: {traitId}");
                return null;
            }

            try
            {
                // Dynamic invocation of Any.Unpack<T>()
                var method = typeof(Any).GetMethod("Unpack").MakeGenericMethod(targetType);
                return (IMessage)method.Invoke(data, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TraitProtobufMapper] Failed to unpack {traitId}: {e.Message}");
                return null;
            }
        }
    }
}
