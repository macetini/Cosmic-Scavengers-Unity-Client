using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CosmicScavengers.Networking.Handlers.Binary;
using UnityEngine;

namespace CosmicScavengers.Networking.Communication
{
    /// <summary>
    /// Central registry for all network command handlers in the scene.
    /// Use the Context Menu "Assign Handlers" to verify registrations in the Inspector.
    /// </summary>
    public class BinaryCommandHandlers : MonoBehaviour
    {
        [Header("Debug View")]
        [Tooltip("Visual list of registered handlers (Read Only)")]
        [SerializeField, ReadOnly(true)]
        private List<string> registeredHandlersCode = new();

        // The actual runtime registry.
        // Note: Dictionaries are not serialized by Unity, so we populate this on Awake.
        private readonly Dictionary<short, IBinaryCommandHandler> handlersByCode = new();

        /// <summary>
        /// Context Menu allows you to trigger this in the editor to see the debug list.
        /// </summary>
        [ContextMenu("Assign Handlers (Preview)")]
        private void AssignHandlersPreview()
        {
            PopulateHandlerDictionary(true);
        }

        void Awake()
        {
            PopulateHandlerDictionary();
        }

        private void PopulateHandlerDictionary(bool isPreview = false)
        {
            handlersByCode.Clear();
            registeredHandlersCode.Clear();

            var foundHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IBinaryCommandHandler>();

            foreach (var handler in foundHandlers)
            {
                short commandCode = (short)handler.Command;
                if (!handlersByCode.ContainsKey(commandCode))
                {
                    handlersByCode.Add(commandCode, handler);
                    registeredHandlersCode.Add($"[{commandCode}] - [{handler.GetType().Name}]");
                }
                else
                {
                    Debug.LogWarning(
                        $"[BinaryCommandHandlers] Duplicate handler for Command {handler.Command} found on {handler.GetType().Name}!"
                    );
                }
            }

            if (isPreview)
            {
                Debug.Log(
                    $"[BinaryCommandHandlers] Preview: Found {handlersByCode.Count} handlers in scene."
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
                    $"[BinaryCommandHandlers] No handler registered for command: {(NetworkBinaryCommand)command}"
                );
            }
        }
    }
}
