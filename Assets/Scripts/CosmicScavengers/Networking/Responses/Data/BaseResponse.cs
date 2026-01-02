using UnityEngine;

namespace CosmicScavengers.Core.Networking.Responses.Data
{
    public class BaseResponse<T> : MonoBehaviour
    {
        public virtual void Handle(T[] parameters) { }
    }
}
