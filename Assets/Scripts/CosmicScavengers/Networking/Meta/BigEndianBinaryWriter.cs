using System;
using System.IO;
using System.Text;

namespace CosmicScavengers.Networking.Meta
{
    /// <summary>
    /// A custom BinaryWriter that ensures multi-byte values are written in Big-Endian byte order,
    /// which is the network standard and Java's default.
    /// </summary>
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream output) : base(output) { }

        public BigEndianBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }

        public override void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            base.Write(bytes);
        }

        public override void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            base.Write(bytes);
        }

        public override void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            base.Write(bytes);
        }

        // You can add overrides for other types like float, double, ushort, uint, ulong if needed.
    }
}