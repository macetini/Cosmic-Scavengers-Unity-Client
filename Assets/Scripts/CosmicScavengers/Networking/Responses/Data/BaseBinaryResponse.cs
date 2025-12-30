using CosmicScavengers.Core.Networking.Request.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public class BaseBinaryResponse : MonoBehaviour, INetworkResponse
    {
        public virtual void Handle(byte[] parameters) { }

        public virtual void OnDestroy() { }
    }
}
