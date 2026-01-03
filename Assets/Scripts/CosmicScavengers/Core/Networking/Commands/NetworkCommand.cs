using System;
using CosmicScavengers.Core.Networking.Commands.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands
{
    /// <summary>
    /// A unified wrapper that can represent either a Binary or Text command.
    /// Supports implicit conversion for seamless usage in method signatures.
    /// </summary>
    [Serializable]
    public readonly struct NetworkCommand : IEquatable<NetworkCommand>
    {
        public NetworkBinaryCommand BinaryCommand { get; }
        public NetworkTextCommand TextCommand { get; }
        public CommandType Type { get; }

        public NetworkCommand(NetworkBinaryCommand binaryCommand)
        {
            BinaryCommand = binaryCommand;
            TextCommand = NetworkTextCommand.UNKNOWN;
            Type = CommandType.BINARY;
        }

        public NetworkCommand(NetworkTextCommand textCommand)
        {
            BinaryCommand = NetworkBinaryCommand.UNKNOWN;
            TextCommand = textCommand;
            Type = CommandType.TEXT;
        }

        public static implicit operator NetworkCommand(NetworkBinaryCommand binaryCommand) =>
            new(binaryCommand);

        public static implicit operator NetworkCommand(NetworkTextCommand textCommand) =>
            new(textCommand);

        public readonly bool Equals(NetworkCommand other)
        {
            return Type == other.Type
                && (
                    Type == CommandType.BINARY
                        ? BinaryCommand == other.BinaryCommand
                        : TextCommand == other.TextCommand
                );
        }

        public override readonly bool Equals(object obj) =>
            obj is NetworkCommand other && Equals(other);

        public override readonly int GetHashCode() =>
            Type switch
            {
                CommandType.BINARY => HashCode.Combine(true, BinaryCommand),
                CommandType.TEXT => HashCode.Combine(false, TextCommand),
                _ => throw new Exception($"Unknown Command Type: {Type}"),
            };

        public override readonly string ToString() =>
            Type switch
            {
                CommandType.BINARY => $"[B] {BinaryCommand}",
                CommandType.TEXT => $"[T] {TextCommand}",
                _ => throw new Exception($"Unknown Command Type: {Type}"),
            };
    }
}
