using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using CosmicScavengers.Networking.Commands.Responses.Data.Text;
using UnityEngine;

namespace CosmicScavengers.Gameplay.Networking.Responses.Data
{
    public class ConnectPassResponse : BaseTextResponse
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Request command chanel for response command.")]
        private RequestChannel requestCommandChannel;

        void Awake()
        {
            if (requestCommandChannel == null)
            {
                Debug.LogError("RequestCommandChannel is not assigned in ConnectPassResponse.");
            }
        }

        public override NetworkTextCommand Command => NetworkTextCommand.S_CONNECT_PASS;

        public override void Handle(string[] parameters)
        {
            if (!Active)
            {
                Debug.Log("[ConnectPassResponse] Handler is inactive. Ignoring message.");
                return;
            }

            Debug.Log(
                "[ConnectPassResponse] Handshake with server completed. Ready for login/register."
            );

            requestCommandChannel.Raise(NetworkTextCommand.C_LOGIN);
        }
    }
}
