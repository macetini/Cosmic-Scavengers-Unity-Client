using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Text.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Derived.Text
{
    /// <summary>
    /// Handles the initial handshake request to the server.
    /// Serializes as: "C_CONNECT|username|password"
    ///
    public class LoginRequest : BaseTextRequest
    {
        protected override NetworkTextCommand Command => NetworkTextCommand.C_LOGIN;

        protected override bool PackParameters(object[] parameters)
        {
            // For now, we use hardcoded credentials.
            string username = "player_1";
            string password = "secret";

            Debug.Log($"[LoginRequest] Packing credentials for: {username}");

            WriteArgs(username, password);

            return true;
        }
    }
}
