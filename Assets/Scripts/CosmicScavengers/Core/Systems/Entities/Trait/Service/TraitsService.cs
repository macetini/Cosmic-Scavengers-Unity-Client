using System;
using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Traits.Service
{
    /// <summary>
    /// A domain service that acts as the final gateway between the Entity/Trait system
    /// and the Networking infrastructure.
    /// </summary>
    public class TraitsService : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        private RequestChannel requestChannel;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("[TraitsService] RequestChannel reference is missing!");
            }
        }

        /// <summary>
        /// Handled by the TraitsProcessor during its LateUpdate sync phase.
        /// Extracts the command and payload, prepends the owner ID, and dispatches to the network.
        /// </summary>
        public void RequestTraitSync(IEntity owner, ITrait trait)
        {
            if (requestChannel == null)
                return;

            object[] traitPayload = trait.GetSyncPayload() ?? Array.Empty<object>();

            object[] finalPayload = new object[traitPayload.Length + 1];
            finalPayload[0] = owner.Id;
            if (traitPayload != null && traitPayload.Length > 0)
            {
                Array.Copy(traitPayload, 0, finalPayload, 1, traitPayload.Length);
            }

            var command = trait.GetSyncCommand();
            requestChannel.Raise(command, traitPayload);
        }

        /// <summary>
        /// Optional: High-level utility to manually force a sync for all traits on an entity.
        /// Useful for 'Dirty' flags or re-syncing after a reconnect.
        /// </summary>
        public void SyncAllTraits(IEntity entity)
        {
            if (entity?.Traits == null)
                return;

            foreach (var trait in entity.Traits)
            {
                RequestTraitSync(entity, trait);
            }
        }
    }
}
