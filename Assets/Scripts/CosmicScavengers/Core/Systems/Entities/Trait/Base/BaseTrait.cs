using System;
using System.ComponentModel;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Systems.Entities.Meta;
using CosmicScavengers.Core.Systems.Entity.Traits.Meta;
using Google.Protobuf;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entity.Traits
{
    /// <summary>
    /// Abstract base class that removes boilerplate for all Entity Traits.
    /// </summary>
    public abstract class BaseTrait : MonoBehaviour, ITrait
    {
        [Header("Visual Feedback")]
        [SerializeField, ReadOnly(true)]
        [Tooltip("The display name of this trait. If empty, the class name will be used.")]
        private string traitName;
        public string Name
        {
            get => string.IsNullOrEmpty(traitName) ? GetType().Name : traitName;
            set => traitName = value;
        }

        public virtual Type GetSystemType()
        {
            return null; //  TODO: Replace with type UNKNOWN or NONE
        }

        public virtual BaseNetworkCommand GetSyncCommand()
        {
            return NetworkBinaryCommand.UNKNOWN;
        }

        /// <summary>
        /// Indicates if the trait has a pending request (Movement, Attack, etc.)
        /// that needs to be synchronized with the server via a Service.
        /// </summary>
        public bool IsPendingSync { get; protected set; } = false;

        /// <summary>
        /// Controls whether the OnUpdate logic should run.
        /// </summary>
        public bool IsEnabled { get; } = true;

        public bool Active { get; protected set; } = true;

        public IEntity Owner
        {
            get => owner;
            set => SetOwner(value);
        }
        private IEntity owner = null;

        public int Priority { get; private set; }
        public int UpdateFrequency { get; private set; }
        public bool PendingUpdate { get; set; }

        public IMessage ProtoData
        {
            set => SetProtoData(value);
        }
        protected IMessage protoData = null;

        /// <summary>
        /// Called by the TraitsUpdater/Service once the sync request has been dispatched.
        /// </summary>
        public void ClearSync()
        {
            IsPendingSync = false;
        }

        public void RequestSync()
        {
            IsPendingSync = true;
        }

        public virtual object[] GetSyncPayload()
        {
            return null;
        }

        public void SetProtoData(IMessage protoData)
        {
            this.protoData =
                protoData ?? throw new ArgumentNullException("ProtoData cannot be null.");

            InitializeData();
        }

        /// <summary>
        /// Optional override for subclasses to perform their own setup logic.
        /// </summary>
        protected abstract void InitializeData();

        public abstract void OnRegister();

        private void SetOwner(IEntity owner)
        {
            if (this.owner != null)
            {
                Debug.LogWarning(
                    $"[BaseTrait] Attempted to set owner for trait {Name} twice. Once assigned the trait can't change Owner."
                );
                return;
            }
            this.owner =
                owner
                ?? throw new ArgumentNullException(
                    "Owner cannot be null for BaseTrait initialization."
                );
        }

        public void OnSpawned() { }

        public bool OwnedBySameEntity(BaseTrait otherTrait)
        {
            return otherTrait != null
                && Owner != null
                && otherTrait.Owner != null
                && Owner.Id == otherTrait.Owner.Id;
        }

        public void LinkSystem(object system)
        {
            throw new NotImplementedException();
        }

        public void UnlinkSystem(object system)
        {
            throw new NotImplementedException();
        }
    }
}
