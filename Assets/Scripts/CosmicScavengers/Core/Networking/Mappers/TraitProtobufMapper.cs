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
        private const string PROTO_METHOD_NAME = "Unpack";
        private const string TRAIT_PROTO_TYPE = "TraitProto";

        public TraitProtobufMapper()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.Namespace == PROTO_NAMESPACE && typeof(IMessage).IsAssignableFrom(type))
                {
                    string traitId = type.Name.Replace(TRAIT_PROTO_TYPE, "").ToLower();
                    typeCache[traitId] = type;
                }
            }
        }

        public IMessage MapFromProto(string traitId, Any data)
        {
            if (string.IsNullOrEmpty(traitId))
            {
                Debug.LogWarning("[TraitProtobufMapper] TraitId is empty.");
                return null;
            }
            if (!typeCache.TryGetValue(traitId.ToLower(), out Type targetType))
            {
                Debug.LogWarning($"[TraitProtobufMapper] No C# class found for TraitId: {traitId}");
                return null;
            }

            try
            {
                var method = typeof(Any).GetMethod(
                    PROTO_METHOD_NAME,
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null
                );

                if (method == null)
                {
                    Debug.LogError("[TraitProtobufMapper] Could not find Unpack method on Any.");
                    return null;
                }

                var genericMethod = method.MakeGenericMethod(targetType);
                return (IMessage)genericMethod.Invoke(data, null);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TraitProtobufMapper] Failed to unpack {traitId}: {e.Message}");
                return null;
            }
        }
    }
}
