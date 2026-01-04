using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices; // Required for Extension methods

namespace CosmicScavengers.Networking.Extensions
{
    /// <summary>
    /// Provides extension methods for System.IO.BinaryReader to support reading
    /// primitive data types in Big Endian (Network Byte Order).
    /// </summary>
    public static class BigEndianBinaryReaderExtensions
    {
        // CompilerServices is used to mark this class's methods as extensions.

        // --- 16-bit Short / Ushort ---

        /// <summary>
        /// Reads a 16-bit signed integer (short) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadInt16BE(this BinaryReader reader)
        {
            // Read 2 bytes, which BinaryReader interprets as LE.
            short networkValue = reader.ReadInt16();
            // Convert from Network (BE) to Host (LE).
            return IPAddress.NetworkToHostOrder(networkValue);
        }

        /// <summary>
        /// Reads a 16-bit signed integer (short) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(this BinaryReader reader)
        {
            return reader.ReadInt16BE();
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer (ushort) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUInt16BE(this BinaryReader reader)
        {
            // Read as signed short for conversion function
            short signedNetworkValue = reader.ReadInt16();
            short signedHostValue = IPAddress.NetworkToHostOrder(signedNetworkValue);
            return unchecked((ushort)signedHostValue);
        }

        // --- 32-bit Integer / Uint ---

        /// <summary>
        /// Reads a 32-bit signed integer (int) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32BE(this BinaryReader reader)
        {
            // Read 4 bytes.
            int networkValue = reader.ReadInt32();
            // Convert from Network (BE) to Host (LE).
            return IPAddress.NetworkToHostOrder(networkValue);
        }

        /// <summary>
        /// Reads a 32-bit signed integer (int) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(this BinaryReader reader)
        {
            return reader.ReadInt32BE();
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer (uint) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32BE(this BinaryReader reader)
        {
            // Read as signed int for conversion function
            int signedNetworkValue = reader.ReadInt32();
            int signedHostValue = IPAddress.NetworkToHostOrder(signedNetworkValue);
            return unchecked((uint)signedHostValue);
        }

        // --- 64-bit Long / Ulong ---

        /// <summary>
        /// Reads a 64-bit signed integer (long) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadInt64BE(this BinaryReader reader)
        {
            // Read 8 bytes.
            long networkValue = reader.ReadInt64();
            // Convert from Network (BE) to Host (LE).
            return IPAddress.NetworkToHostOrder(networkValue);
        }

        /// <summary>
        /// Reads a 64-bit signed integer (long) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLong(this BinaryReader reader)
        {
            return reader.ReadInt64BE();
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer (ulong) and converts it from
        /// Big Endian (Network Order) to Host (Little Endian) order.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadUInt64BE(this BinaryReader reader)
        {
            // Read as signed long for conversion function
            long signedNetworkValue = reader.ReadInt64();
            long signedHostValue = IPAddress.NetworkToHostOrder(signedNetworkValue);
            return unchecked((ulong)signedHostValue);
        }

        /// <summary>
        /// Reads a 32-bit floating point number (float) in Big Endian (Network Order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat32BE(this BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (bytes.Length < 4)
            {
                throw new EndOfStreamException("Unable to read 4 bytes for a float value.");
            }

            // Reverse the byte array to convert from BE to LE.
            Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads a 32-bit floating point number (float) in Big Endian (Network Order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat(this BinaryReader reader)
        {
            return reader.ReadFloat32BE();
        }
    }
}
