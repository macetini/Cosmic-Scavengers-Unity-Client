using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Text
{
    public class ConnectRequest : BaseTextRequest
    {
        public override NetworkTextCommand Command => NetworkTextCommand.C_CONNECT;

        public override void Execute(object[] data)
        {
            if (!Active)
            {
                Debug.Log("[ConnectPassHandler] Handler is inactive. Ignoring message.");
                return;
            }

            Debug.Log("[ConnectPassHandler] Sending login credentials to server.");
            string username = "player_1";
            string password = "secret";

            Debug.Log($"[ConnectPassHandler] Username: {username}, Password: {password}");

            CommandChannel.Raise($"{NetworkTextCommand.C_LOGIN}|{username}|{password}");
        }
    }
}
