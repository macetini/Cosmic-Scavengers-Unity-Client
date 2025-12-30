using CosmicScavengers.Core.Networking.Request.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public class BaseBinaryResponse : MonoBehaviour, INetworkResponse
    {
        public void Execute(params object[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new System.NotImplementedException();
        }
    }
}
