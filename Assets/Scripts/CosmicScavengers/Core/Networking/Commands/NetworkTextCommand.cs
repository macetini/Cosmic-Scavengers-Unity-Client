using System.ComponentModel;

namespace CosmicScavengers.Core.Networking.Commands
{
    /// <summary>
    /// Defines the command types for network messages.
    /// </summary>
    public enum NetworkTextCommand
    {
        [Description("Unknown command received.")]
        UNKNOWN,

        [Description("Client initiating connection handshake.")]
        C_CONNECT,

        [Description("Server Handshake Complete. Ready for Login/Register.")]
        S_CONNECT_PASS,

        [Description("Registration successful. ")]
        S_REGISTER_OK,

        [Description("Indicates a failed registration attempt.")]
        S_REGISTER_FAIL,

        [Description("Initiates a login attempt.")]
        C_LOGIN,

        [Description("Indicates a successful login attempt.")]
        S_LOGIN_PASS,

        [Description("Indicates a failed login attempt.")]
        S_LOGIN_FAIL,
    }
}
