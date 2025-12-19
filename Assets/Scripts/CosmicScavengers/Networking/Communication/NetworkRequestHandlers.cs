using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CosmicScavengers.Networking.Handlers;
using UnityEngine;

namespace CosmicScavengers.Networking.Communication
{
    /// <summary>
    /// Central registry for all network command handlers in the scene.
    /// Use the Context Menu "Assign Handlers" to verify registrations in the Inspector.
    /// </summary>
    public class NetworkCommandResponseHandlers : MonoBehaviour
    {
        [Header("Debug View")]
        [Tooltip("Visual list of registered handlers (Read Only)")]
        [SerializeField, ReadOnly(true)]
        private List<string> registeredHandlersCode = new();

        // The actual runtime registry.
        // Note: Dictionaries are not serialized by Unity, so we populate this on Awake.
        private readonly Dictionary<short, INetworkRequestHandler> handlersByCode = new();
        private static NetworkCommandResponseHandlers instance;

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
            handlersByCode.Clear();
            registeredHandlersCode.Clear();

            var foundHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<INetworkRequestHandler>();

            foreach (var handler in foundHandlers)
            {
                short commandCode = (short)handler.CommandCode;
                if (!handlersByCode.ContainsKey(commandCode))
                {
                    handlersByCode.Add(commandCode, handler);
                    registeredHandlersCode.Add($"Code {commandCode}: {handler.GetType().Name}");
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
                    $"[NetworkCommandHandlers] Preview: Found {handlersByCode.Count} handlers in scene."
                );
            }
        }

        /// <summary>
        /// Routes incoming protobuf data to the correct handler.
        /// </summary>
        public void Handle(short command, byte[] data)
        {
            if (handlersByCode.TryGetValue(command, out var handler))
            {
                handler.Handle(data);
            }
            else
            {
                Debug.LogWarning(
                    $"[NetworkCommandHandlers] No handler registered for command code: {command}"
                );
            }
        }
    }
}
