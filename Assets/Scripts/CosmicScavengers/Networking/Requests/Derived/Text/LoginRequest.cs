using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Text.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Text
{
    public class LoginRequest : BaseTextRequest
    {
        protected override NetworkTextCommand Command => NetworkTextCommand.C_LOGIN;

        public override void Execute(string[] data)
        {
            if (!Active)
            {
                Debug.Log("[LoginRequest] Handler is inactive. Ignoring message.");
                return;
            }

            string username = "player_1";
            string password = "secret";

            Debug.Log(
                $"[LoginRequest] Executing login request for user: {username} with password: {password}"
            );

            Data = new string[2];
            Data[0] = username;
            Data[1] = password;

            Raise();
        }
    }
}
