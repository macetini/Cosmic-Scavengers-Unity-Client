using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Text.Data;

namespace CosmicScavengers.Networking.Requests.Derived.Text
{
    /// <summary>
    /// Handles the initial handshake request to the server.
    /// Serializes as: "C_CONNECT"
    /// </summary>
    public class ConnectRequest : BaseTextRequest
    {
        protected override NetworkTextCommand Command => NetworkTextCommand.C_CONNECT;
    }
}
