using CosmicScavengers.Networking.Channel;
using CosmicScavengers.Networking.Commands.Data.Binary;
using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Responses.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Data
{
    public class LoginPassResponse : BaseTextResponse
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Request command chanel for response command.")]
        private RequestChannel requestChannel;

        public override NetworkTextCommand Command => NetworkTextCommand.S_LOGIN_PASS;

        void Awake()
        {
            if (requestChannel == null)
            {
                Debug.LogError("RequestCommandChannel is not assigned in LoginPassResponse.");
            }
        }

        public override void Handle(string[] data)
        {
            if (!Active)
            {
                Debug.Log("[LoginPassHandler] Handler is inactive. Ignoring message.");
                return;
            }
            if (data.Length < 1)
            {
                Debug.LogError(
                    "[LoginPassHandler] Invalid data received. Expected at least 1 parameter."
                );
                return;
            }
            if (!long.TryParse(data[0], out long playerId))
            {
                Debug.LogError(
                    "[LoginPassHandler] Failed to parse Player ID from data: " + data[0]
                );
                return;
            }

            InitPlayerData(playerId);
        }

        private void InitPlayerData(long playerId)
        {
            Debug.Log("[LoginPassHandler] Initializing player data for Player ID: " + playerId);
            requestChannel.Raise(NetworkBinaryCommand.REQUEST_WORLD_STATE_C, playerId);
        }
    }
}
