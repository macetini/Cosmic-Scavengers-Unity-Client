using System.Collections.Generic;
using CosmicScavengers.Networking.Commands.Binary;
using CosmicScavengers.Networking.Request.Binary.Data;
using CosmicScavengers.Networking.Requests.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Networking.Request.Registry
{
    [CreateAssetMenu(menuName = "Registry/BinaryRequestRegistry")]
    public class BinaryRequestRegistry : ScriptableObject
    {
        public List<BinaryRequestEntry> Entries = new();
        private readonly Dictionary<NetworkBinaryCommand, BaseBinaryRequest> lookUp = new();

        public BaseBinaryRequest GetPrefab(NetworkBinaryCommand key)
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
                        $"[BinaryRequestRegistry] Entry for Command {entry.Command} has a null Request Prefab."
                    );
                    continue;
                }
                lookUp[entry.Command] = entry.Prefab;
            }
        }
    }
}
