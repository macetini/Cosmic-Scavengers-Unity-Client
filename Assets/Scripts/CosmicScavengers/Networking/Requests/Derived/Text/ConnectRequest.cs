using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Text
{
    public class ConnectRequest : BaseTextRequest
    {
        protected override NetworkTextCommand Command => NetworkTextCommand.C_CONNECT;

        public void Execute(string data)
        {
            if (!Active)
            {
                Debug.Log("[ConnectRequest] Handler is inactive. Ignoring message.");
                return;
            }

            string username = "player_1";
            string password = "secret";

            Debug.Log(
                $"[ConnectRequest] Executing connect request for user: {username} with password: {password}"
            );

            Data = $"{username}|{password}";
            Raise();
        }
    }
}
