using CosmicScavengers.Networking.Commands.Text;
using CosmicScavengers.Networking.Requests.Channels;
using UnityEngine;

namespace CosmicScavengers.Networking.Responses.Data
{
    public class ConnectPassResponse : BaseTextResponse
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Request command chanel for response command.")]
        private RequestCommandChannel requestCommandChannel;

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
            Debug.Log(
                "[ConnectPassResponse] Handshake with server completed. Ready for login/register."
            );

            requestCommandChannel.Raise(NetworkTextCommand.C_LOGIN, "kkkk");
        }
    }
}
