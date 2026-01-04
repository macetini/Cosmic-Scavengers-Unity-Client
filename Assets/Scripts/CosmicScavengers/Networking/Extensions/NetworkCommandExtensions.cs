using System;
using CosmicScavengers.Networking.Commands.Data.Text;

namespace CosmicScavengers.Networking.Extensions
{
    public static class NetworkCommandExtensions
    {
        public static NetworkTextCommand ToNetworkCommand(
            this string commandString,
            bool ignoreCase = true
        )
        {
            if (string.IsNullOrEmpty(commandString))
            {
                return NetworkTextCommand.UNKNOWN;
            }
            if (Enum.TryParse<NetworkTextCommand>(commandString, ignoreCase, out var result))
            {
                return result;
            }
            return NetworkTextCommand.UNKNOWN;
        }
    }
}
