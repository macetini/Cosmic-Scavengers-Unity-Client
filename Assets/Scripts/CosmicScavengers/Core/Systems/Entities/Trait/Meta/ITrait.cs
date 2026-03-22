using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Systems.Entities.Meta;
using Google.Protobuf;

namespace CosmicScavengers.Core.Systems.Entity.Traits.Meta
{
    /// <summary>
    /// Base interface for all Entity Traits.
    /// Formalizes the contract for initialization, simulation, and networking state.
    /// </summary>
    public interface ITrait
    {
        // --------------------------------
        // --- Identity & Lifecycle -------
        // --------------------------------

        /// <summary>
        /// The name of the trait.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The Entity that owns the trait.
        /// </summary>
        IEntity Owner { get; set; }

        /// <summary>
        /// The ProtoData for the trait.
        /// </summary>
        IMessage ProtoData { set; }

        /// <summary>
        /// Controls whether the OnUpdate logic is active.
        /// </summary>
        bool IsEnabled { get; }

        // --------------------------------
        // --- The 1:1 System Handshake ---
        // --------------------------------

        System.Type GetSystemType();

        // ----------------------------------
        // --- Networking State (Minimal) ---
        // ----------------------------------

        /// <summary>
        /// Initializes the trait with its owner entity and configuration data.
        /// </summary>
        /// <param name="owner">Entity that owns the trait.</param>
        /// <param name="config">Trait config data to be parsed.</param>
        /// <summary>
        /// Flag indicating a request is waiting to be sent to the server.
        /// </summary>
        bool IsPendingSync { get; }

        /// <summary>
        /// Resets the IsPendingSync flag.
        /// Called by systems after the sync request has been handled.
        /// </summary>
        void ClearSync();

        // REFACTOR

        int Priority { get; }
        int UpdateFrequency { get; }
        bool PendingUpdate { get; set; }

        void OnRegister();
        bool Active { get; }

        /// <summary>
        /// Called by the EntityOrchestrator when the trait is spawned.
        /// </summary>
        void OnSpawned();

        /// <summary>
        /// Returns the network sync command. The command can be TEXT or BINARY,
        /// but will in most cases be Binary. Used by the TraitsService to dispatch
        ///  the command to the network.
        /// </summary>
        BaseNetworkCommand GetSyncCommand();

        /// <summary>
        /// Returns the sync command and payload for the trait.
        /// </summary>
        object[] GetSyncPayload();

        /// <summary>
        /// The main simulation loop for the trait.
        /// </summary>
        //void OnUpdate(float deltaTime);
    }
}
