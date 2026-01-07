using UnityEngine;

namespace CosmicScavengers.Networking.Commands.Responses.Data
{
    public class BaseResponse<T> : MonoBehaviour
    {
        [Header("Status")]
        public bool Active = true;

        public virtual void Handle(T[] parameters) { }
    }
}
