using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Text.Data;
using CosmicScavengers.Networking.Requests.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Registry
{
    [CreateAssetMenu(menuName = "Registry/TextRequestRegistry")]
    public class TextRequestRegistry : ScriptableObject
    {
        public List<TextRequestEntry> Entries = new();
        private readonly Dictionary<NetworkTextCommand, BaseTextRequest> lookUp = new();

        public BaseTextRequest GetPrefab(NetworkTextCommand key)
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
                        $"[TextRequestRegistry] Entry for Command {entry.Command} has a null Request Prefab."
                    );
                    continue;
                }
                lookUp[entry.Command] = entry.Prefab;
            }
        }
    }
}
