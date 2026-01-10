using System;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Commands.Data.Meta;
using CosmicScavengers.Core.Networking.Commands.Data.Text;

namespace CosmicScavengers.Core.Networking.Commands.Data
{
    /// <summary>
    /// A unified wrapper that can represent either a Binary or Text command.
    /// Supports implicit conversion for seamless usage in method signatures.
    /// </summary>
    [Serializable]
    public readonly struct BaseNetworkCommand : IEquatable<BaseNetworkCommand>
    {
        public NetworkBinaryCommand BinaryCommand { get; }
        public NetworkTextCommand TextCommand { get; }
        public CommandType Type { get; }        

        public BaseNetworkCommand(NetworkBinaryCommand binaryCommand)
        {
            BinaryCommand = binaryCommand;
            TextCommand = NetworkTextCommand.UNKNOWN;
            Type = CommandType.BINARY;
        }

        public BaseNetworkCommand(NetworkTextCommand textCommand)
        {
            BinaryCommand = NetworkBinaryCommand.UNKNOWN;
            TextCommand = textCommand;
            Type = CommandType.TEXT;
        }

        public static implicit operator BaseNetworkCommand(NetworkBinaryCommand binaryCommand) =>
            new(binaryCommand);

        public static implicit operator BaseNetworkCommand(NetworkTextCommand textCommand) =>
            new(textCommand);

        public readonly bool Equals(BaseNetworkCommand other)
        {
            return Type == other.Type
                && (
                    Type == CommandType.BINARY
                        ? BinaryCommand == other.BinaryCommand
                        : TextCommand == other.TextCommand
                );
        }

        public override readonly bool Equals(object obj) =>
            obj is BaseNetworkCommand other && Equals(other);

        public override readonly int GetHashCode()
        {
            return Type switch
            {
                CommandType.BINARY => HashCode.Combine(true, BinaryCommand),
                CommandType.TEXT => HashCode.Combine(false, TextCommand),
                _ => throw new Exception($"Unknown Command Type: {Type}"),
            };
        }

        public override readonly string ToString()
        {
            return Type switch
            {
                CommandType.BINARY => $"[{CommandType.BINARY}][{BinaryCommand}]",
                CommandType.TEXT => $"[{CommandType.TEXT}][{TextCommand}]",
                _ => throw new Exception($"Unknown Command Type: {Type}"),
            };
        }
    }
}
