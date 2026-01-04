using System.Collections.Generic;
using CosmicScavengers.Networking.Commands.Binary;
using CosmicScavengers.Networking.Responses.Data;
using CosmicScavengers.Networking.Responses.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Networking.Response.Registry
{
    [CreateAssetMenu(menuName = "Registry/BinaryResponseRegistry")]
    public class BinaryResponseRegistry : ScriptableObject
    {
        public List<BinaryResponseEntry> Entries = new();
        private readonly Dictionary<NetworkBinaryCommand, BaseBinaryResponse> lookUp = new();

        public BaseBinaryResponse GetPrefab(NetworkBinaryCommand key)
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
                        $"[BinaryResponseRegistry] Entry for Command {entry.Command} has a null Response Prefab."
                    );
                    continue;
                }
                lookUp[entry.Command] = entry.Prefab;
            }
        }
    }
}
