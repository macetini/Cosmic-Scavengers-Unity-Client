using System.Collections.Generic;
using CosmicScavengers.Networking.Requests.Registry.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Request.Registry
{
    [CreateAssetMenu(menuName = "Registry/BinaryRequestRegistry")]
    public class BinaryRequestRegistry : ScriptableObject
    {
        public List<BinaryRequestEntry> Entries = new();
        private readonly Dictionary<short, BinaryRequestEntry> lookUp = new();
    }
}
