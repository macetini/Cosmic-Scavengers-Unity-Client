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
        /// Controls whether this trait is allowed to run during system ticks.
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
        /// Flag indicating a request is waiting to be sent to the server.
        /// </summary>
        bool IsPendingSync { get; }

        /// <summary>
        /// Resets the IsPendingSync flag.
        /// Called by systems after the sync request has been handled.
        /// </summary>
        void ClearSync();

        /// <summary>
        /// Priority used by processors to order trait execution.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Number of frames between updates for throttled processing.
        /// </summary>
        int UpdateFrequency { get; }

        /// <summary>
        /// Set when this trait is scheduled to run on the current tick.
        /// </summary>
        bool PendingUpdate { get; set; }

        /// <summary>
        /// True when this trait should run on the current tick.
        /// </summary>
        bool ShouldTickNow { get; }

        void OnRegister();

        /// <summary>
        /// Optional runtime state used to pause or resume trait behavior.
        /// </summary>
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

        // Trait simulation runs through IGameSystem.OnTick.
    }
}
