using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Mappers;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entities.Traits.Registry;
using CosmicScavengers.Core.Systems.Entity.Traits;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using CosmicScavengers.Networking.Protobuf.Traits;
using Google.Protobuf;
using Google.Protobuf.Collections;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Trait.Factory
{
    public class TraitFactory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TraitRegistry traitRegistry;
        private readonly TraitProtobufMapper mapper = new();

        private const string TRAIT_PROTO_TYPE = "TraitProto";

        void Awake()
        {
            if (traitRegistry == null)
            {
                Debug.LogError("[TraitFactory] TraitRegistry reference is missing!");
            }
        }

        public List<IMessage> ParseTraitProtoData(RepeatedField<TraitInstanceProto> traitProtos)
        {
            List<IMessage> traitsProtoData = new(traitProtos.Count);
            foreach (var traitInstance in traitProtos)
            {
                string traitId = traitInstance.TraitId;

                var protoData = mapper.MapFromProto(traitId, traitInstance.Data);
                if (protoData == null)
                {
                    Debug.LogWarning(
                        $"[TraitFactory] Failed to unpack TraitId: '{traitId}' skipping."
                    );
                    continue;
                }
                traitsProtoData.Add(protoData);
            }
            traitsProtoData.TrimExcess();
            return traitsProtoData;
        }

        public List<ITrait> CreateAndAttachTraits(
            List<IMessage> traitsProtoData,
            Transform traitsContainer
        )
        {
            List<ITrait> traits = new(traitsProtoData.Count);
            foreach (var protoData in traitsProtoData)
            {
                string traitId = protoData.Descriptor.Name.Replace(TRAIT_PROTO_TYPE, "").ToLower();
                BaseTrait prefab = traitRegistry.GetPrefab(traitId);
                if (prefab == null)
                {
                    Debug.LogWarning($"[TraitFactory] No prefab found for TraitId: {traitId}");
                    continue;
                }

                ITrait newTrait = Instantiate(prefab, traitsContainer);
                newTrait.ProtoData = protoData;
                newTrait.OnSpawned();

                traits.Add(newTrait);
            }

            return traits;
        }
    }
}
