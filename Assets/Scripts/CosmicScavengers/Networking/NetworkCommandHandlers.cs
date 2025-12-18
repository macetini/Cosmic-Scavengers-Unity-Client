using System.Collections.Generic;
using System.Linq;
using CosmicScavengers.Networking.Handlers;
using UnityEngine;

namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Central registry for all network command handlers in the scene.
    /// Use the Context Menu "Assign Handlers" to verify registrations in the Inspector.
    /// </summary>
    public class NetworkCommandHandlers : MonoBehaviour
    {
        [Header("Debug View")]
        [Tooltip("Visual list of registered handlers (Read Only)")]
        [SerializeField]
        private List<string> activeHandlerCodes = new();

        // The actual runtime registry.
        // Note: Dictionaries are not serialized by Unity, so we populate this on Awake.
        private readonly Dictionary<short, INetworkCommandHandler> handlers = new();

        private static NetworkCommandHandlers instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                PopulateHandlerDictionary();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Context Menu allows you to trigger this in the editor to see the debug list.
        /// </summary>
        [ContextMenu("Assign Handlers (Preview)")]
        private void AssignHandlersPreview()
        {
            PopulateHandlerDictionary(true);
        }

        private void PopulateHandlerDictionary(bool isPreview = false)
        {
            handlers.Clear();
            activeHandlerCodes.Clear();

            var foundHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<INetworkCommandHandler>();

            foreach (var handler in foundHandlers)
            {
                if (!handlers.ContainsKey(handler.CommandCode))
                {
                    handlers.Add(handler.CommandCode, handler);
                    activeHandlerCodes.Add($"Code {handler.CommandCode}: {handler.GetType().Name}");
                }
                else
                {
                    Debug.LogWarning(
                        $"[NetworkCommandHandlers] Duplicate handler for Command {handler.CommandCode} found on {handler.GetType().Name}!"
                    );
                }
            }

            if (isPreview)
            {
                Debug.Log(
                    $"[NetworkCommandHandlers] Preview: Found {handlers.Count} handlers in scene."
                );
            }
        }

        /// <summary>
        /// Routes incoming protobuf data to the correct handler.
        /// </summary>
        public void HandleCommand(short commandCode, byte[] data)
        {
            if (handlers.TryGetValue(commandCode, out var handler))
            {
                handler.Handle(data);
            }
            else
            {
                Debug.LogWarning(
                    $"[NetworkCommandHandlers] No handler registered for command code: {commandCode}"
                );
            }
        }
    }
}
