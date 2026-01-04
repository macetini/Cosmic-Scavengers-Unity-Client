using System.Collections.Generic;
using CosmicScavengers.Networking.Responses.Data;
using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Responses.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Networking.Response.Registry
{
    [CreateAssetMenu(menuName = "Registry/TextResponseRegistry")]
    public class TextResponseRegistry : ScriptableObject
    {
        public List<TextResponseEntry> Entries = new();
        private readonly Dictionary<NetworkTextCommand, BaseTextResponse> lookUp = new();

        public BaseTextResponse GetPrefab(NetworkTextCommand key)
        {
            if (lookUp.Count == 0)
            {
                InitializeLookup();
            }
            return lookUp.TryGetValue(key, out var prefab) ? prefab : null;
        }

        private void InitializeLookup()
        {
            lookUp.Clear();
            foreach (var entry in Entries)
            {
                if (entry.Prefab == null)
                {
                    Debug.LogWarning(
                        $"[TextResponseRegistry] Entry for Command {entry.Command} has a null Response Prefab."
                    );
                    continue;
                }
                lookUp[entry.Command] = entry.Prefab;
            }
        }
    }
}
