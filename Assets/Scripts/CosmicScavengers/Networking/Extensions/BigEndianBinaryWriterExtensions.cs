using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

namespace CosmicScavengers.Networking.Extensions
{
    /// <summary>
    /// Provides extension methods for System.IO.BinaryWriter to support writing
    /// primitive data types in Big Endian (Network Byte Order).
    /// This converts the host's native Little Endian order into Network Byte Order.
    /// </summary>
    public static class BigEndianBinaryWriterExtensions
    {
        // --- 16-bit Short / Ushort ---

        /// <summary>
        /// Writes a 16-bit signed integer (short) by converting it from 
        /// Host (Little Endian) to Big Endian (Network Order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt16BE(this BinaryWriter writer, short hostValue)
        {
            // Convert from Host (LE) to Network (BE)
            short networkValue = IPAddress.HostToNetworkOrder(hostValue);
            // Write the converted value (Netty/Java will read this as BE)
            writer.Write(networkValue);
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer (ushort).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt16BE(this BinaryWriter writer, ushort hostValue)
        {
            // Unsigned types must be handled via their signed counterparts for IPAddress conversion
            short signedNetworkValue = IPAddress.HostToNetworkOrder(unchecked((short)hostValue));
            writer.Write(signedNetworkValue);
        }


        // --- 32-bit Integer / Uint ---

        /// <summary>
        /// Writes a 32-bit signed integer (int) by converting it from 
        /// Host (Little Endian) to Big Endian (Network Order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt32BE(this BinaryWriter writer, int hostValue)
        {
            // Convert from Host (LE) to Network (BE)
            int networkValue = IPAddress.HostToNetworkOrder(hostValue);
            // Write the converted value
            writer.Write(networkValue);
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer (uint).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt32BE(this BinaryWriter writer, uint hostValue)
        {
            int signedNetworkValue = IPAddress.HostToNetworkOrder(unchecked((int)hostValue));
            writer.Write(signedNetworkValue);
        }


        // --- 64-bit Long / Ulong ---

        /// <summary>
        /// Writes a 64-bit signed integer (long) by converting it from 
        /// Host (Little Endian) to Big Endian (Network Order).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteInt64BE(this BinaryWriter writer, long hostValue)
        {
            // Convert from Host (LE) to Network (BE)
            long networkValue = IPAddress.HostToNetworkOrder(hostValue);
            // Write the converted value
            writer.Write(networkValue);
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer (ulong).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteUInt64BE(this BinaryWriter writer, ulong hostValue)
        {
            long signedNetworkValue = IPAddress.HostToNetworkOrder(unchecked((long)hostValue));
            writer.Write(signedNetworkValue);
        }

        /// <summary>
        /// Writes a 32-bit floating point number (float) in Big Endian (Network Order).    
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteFloat32BE(this BinaryWriter writer, float value)
        {
            byte[] floatBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(floatBytes);
            }
            writer.Write(floatBytes);
        }
    }
}