using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public class BaseBinaryResponse : MonoBehaviour
    {
        public virtual void Handle(byte[] parameters) { }

        public virtual void OnDestroy() { }
    }
}
