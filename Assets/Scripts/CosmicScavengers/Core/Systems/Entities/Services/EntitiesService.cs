using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using UnityEngine;

namespace CosmicScavengers.Core.Systems.Entities.Services
{
    public class EntitiesServices : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        private RequestChannel requestChannel;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("[EntitiesServices] RequestChannel reference is missing!");
            }
        }
    }
}
