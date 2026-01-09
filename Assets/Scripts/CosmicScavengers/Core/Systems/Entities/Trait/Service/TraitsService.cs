using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Traits.Service
{
    public class TraitsService : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        private RequestChannel requestChannel;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("[TraitsService] RequestChannel reference is missing!");
            }
        }
    }
}
