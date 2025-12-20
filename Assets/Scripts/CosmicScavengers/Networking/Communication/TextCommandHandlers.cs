using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CosmicScavengers.Networking.Handlers.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Communication
{
    public class TextCommandHandlers : MonoBehaviour
    {
        [Header("Debug View")]
        [Tooltip("Visual list of registered handlers (Read Only)")]
        [SerializeField, ReadOnly(true)]
        private List<string> registeredHandlersCode = new();

        private readonly Dictionary<string, ITextCommandHandler> handlersByCode = new();

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
                .OfType<ITextCommandHandler>();

            foreach (var handler in foundHandlers)
            {
                string commandCode = handler.Command.ToString();
                if (!handlersByCode.ContainsKey(commandCode))
                {
                    handlersByCode.Add(commandCode, handler);
                    registeredHandlersCode.Add($"[{commandCode}] - [{handler.GetType().Name}]");
                }
                else
                {
                    Debug.LogWarning(
                        $"[TextCommandHandlers] Duplicate handler for Command {handler.Command} found on {handler.GetType().Name}!"
                    );
                }
            }

            if (isPreview)
            {
                Debug.Log(
                    $"[TextCommandHandlers] Preview: Found {handlersByCode.Count} handlers in scene."
                );
            }
        }

        public void Handle(string commandCode, string[] data)
        {
            if (handlersByCode.TryGetValue(commandCode, out var handler))
            {
                handler.Handle(data);
            }
            else
            {
                Debug.LogWarning(
                    $"[TextCommandHandlers] No handler registered for command code: {commandCode}"
                );
            }
        }
    }
}
