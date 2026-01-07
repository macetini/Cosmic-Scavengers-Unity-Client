using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Request.Data.Text;

namespace CosmicScavengers.Gameplay.Networking.Requests.Derived.Text
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
