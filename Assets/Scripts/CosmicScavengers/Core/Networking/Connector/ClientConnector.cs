using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CosmicScavengers.Core.Networking.Commands.Data.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Connector
{
    /// <summary>
    /// Manages low-level TCP connection to the multiplexed server.
    /// Handles sending and receiving of both text and binary messages.
    /// </summary>
    public class ClientConnector : MonoBehaviour
    {
        // --- Server Connection Details --- // TODO - Move to config file
        private const string HOST = "127.0.0.1";
        private const int PORT = 8080;
        private const int LENGTH_FIELD_SIZE = 4; // 4 bytes for length prefix
        private const int MAX_MESSAGE_SIZE = 1024 * 1024; // 1 MB

        // -----------------------------------

        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;
        private readonly object streamLock = new();

        // Thread-safe queues to hold messages received from the server
        private readonly Queue<string> incomingTextMessages = new();
        private readonly Queue<byte[]> incomingBinaryMessages = new();

        /// Events for incoming messages
        internal event Action<string> OnTextMessageReceived;
        internal event Action<byte[]> OnBinaryMessageReceived;
        internal event Action OnConnected;
        internal event Action OnDisconnected;

        /// <summary>
        /// Indicates whether the client is currently connected to the server.
        /// </summary>
        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        void Start()
        {
            clientThread = new Thread(ConnectToServer) { IsBackground = true };
            clientThread.Start();

            OnConnected?.Invoke(); // TODO - Find a better way to dispatch connection event.
        }

        /// <summary>
        /// Runs in a separate thread. Establishes connection and manages the read loop.
        /// </summary>
        void ConnectToServer()
        {
            try
            {
                client = new TcpClient(HOST, PORT);
                stream = client.GetStream();
                if (!client.Connected)
                {
                    Debug.LogError("[Connector Error] Failed to connect to server.");
                    return;
                }
                Debug.Log("[Connector] Successfully connected to the multiplexed server!");

                while (client.Connected)
                {
                    ReadNextMessage();
                }
            }
            catch (SocketException e)
            {
                Debug.LogError(
                    $"[Connector Error] Connection failed (Socket Exception): {e.Message}. Is the server running on {HOST}:{PORT}?"
                );
            }
            catch (ThreadAbortException)
            {
                Debug.Log("[Connector] Client thread manually aborted.");
            }
            catch (IOException e)
            {
                Debug.LogError($"[Connector Error] Connection lost (IO Exception): {e.Message}");
            }
            finally
            {
                Cleanup();
            }
        }

        /// <summary>
        /// Reads one complete message from the stream (Header + Payload).
        /// Protocol Order: [4-byte Length Prefix] + [1-byte Type] + [Payload Data]
        /// </summary>
        private void ReadNextMessage()
        {
            if (stream == null || !stream.CanRead)
            {
                throw new IOException("Network stream is not readable.");
            }

            byte[] lengthBytes = new byte[LENGTH_FIELD_SIZE];
            try
            {
                // Read the 4-byte length prefix
                if (ReadExactly(stream, lengthBytes, 0, LENGTH_FIELD_SIZE) == 0)
                {
                    throw new IOException("[Connector Error] Stream closed by server.");
                }

                // Convert Big-Endian (Network Order) bytes to local machine Endian (Host Order).
                int networkOrderValue = BitConverter.ToInt32(lengthBytes, 0);
                int messageLength = IPAddress.NetworkToHostOrder(networkOrderValue);

                if (messageLength <= 0 || messageLength > MAX_MESSAGE_SIZE)
                {
                    Debug.LogError(
                        $"[Connector Error] Invalid message length received: {messageLength}."
                    );
                    client.Close();
                    return;
                }

                // Read Payload (L bytes, including the 1-byte Type prefix)
                byte[] fullPayloadBytes = new byte[messageLength];
                ReadExactly(stream, fullPayloadBytes, 0, messageLength);

                // Extract Type and Payload Data
                CommandType messageType = (CommandType)fullPayloadBytes[0];

                int payloadDataLength = messageLength - 1;
                if (payloadDataLength < 0)
                {
                    Debug.LogError(
                        "[Connector Error] Payload data length is negative. Discarding message."
                    );
                    return;
                }

                byte[] payloadData = new byte[payloadDataLength];
                Array.Copy(fullPayloadBytes, 1, payloadData, 0, payloadDataLength);

                if (messageType == CommandType.TEXT)
                {
                    string message = Encoding.UTF8.GetString(payloadData).Trim();
                    lock (incomingTextMessages)
                    {
                        incomingTextMessages.Enqueue(message);
                    }
                }
                else if (messageType == CommandType.BINARY)
                {
                    lock (incomingBinaryMessages)
                    {
                        incomingBinaryMessages.Enqueue(payloadData);
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"[Connector] Received unknown message type: {messageType}. Discarding payload."
                    );
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Connector Error] Reading failed: {ex.Message}");
                Cleanup();
            }
        }

        /// <summary>
        /// Reads exactly 'count' bytes from the stream into the buffer, handling partial reads.
        /// Throws IOException if the stream is closed before reading the requested bytes.
        /// </summary>
        private static int ReadExactly(
            NetworkStream targetStream,
            byte[] buffer,
            int offset,
            int count
        )
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = targetStream.Read(
                    buffer,
                    offset + totalBytesRead,
                    count - totalBytesRead
                );
                if (bytesRead == 0)
                {
                    throw new IOException("Stream closed by server.");
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// Sends raw binary data, automatically framing it with MessageType.BINARY.
        /// Used by ClientGameState for high-frequency updates.
        /// </summary>
        public void DispatchMessage(byte[] buffer, int bufferLength, CommandType type)
        {
            if (buffer == null || buffer.Length == 0 || bufferLength <= 0)
            {
                Debug.LogWarning("Cannot send empty binary data.");
                return;
            }
            SendTypedMessage(buffer, bufferLength, type);
        }

        /// <summary>
        /// Core method for sending messages, constructing the full frame:
        /// [4-byte Length] + [1-byte Type] + [Payload]
        /// </summary>
        private void SendTypedMessage(byte[] buffer, int bufferLength, CommandType type)
        {
            if (!IsConnected || stream == null)
            {
                Debug.LogError($"[Connector] Cannot send {type} message: Not connected to server.");
                return;
            }
            Debug.Log($"[Connector] Sending {type} message of length {bufferLength} bytes.");

            try
            {
                // Payload length = 1 (Type byte) + Buffer N length
                int totalPacketLength = sizeof(byte) + bufferLength;
                // Convert to Network Byte Order (Big Endian)
                byte[] lengthBytes = BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(totalPacketLength)
                );

                lock (streamLock)
                {
                    // 1. Write 4-byte Length Prefix (Big-Endian)
                    stream.Write(lengthBytes, 0, lengthBytes.Length);
                    // 2. Write 1-byte Message Type
                    stream.WriteByte((byte)type);
                    // 3. Write the Payload Data
                    stream.Write(buffer, 0, bufferLength);
                    // 4. Flush
                    stream.Flush();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Connector Error] Sending failed: {ex.Message}");
            }
        }

        void OnApplicationQuit()
        {
            Cleanup();
            OnDisconnected?.Invoke();
        }

        private void Cleanup()
        {
            if (clientThread != null && clientThread.IsAlive)
            {
                clientThread.Interrupt();
            }

            stream?.Close();
            client?.Close();
        }

        /// <summary>
        /// Executes on the main Unity thread to process queued messages.
        /// </summary>
        void Update()
        {
            lock (incomingTextMessages)
            {
                while (incomingTextMessages.Count > 0)
                {
                    string data = incomingTextMessages.Dequeue();
                    OnTextMessageReceived?.Invoke(data);
                }
            }

            lock (incomingBinaryMessages)
            {
                while (incomingBinaryMessages.Count > 0)
                {
                    byte[] data = incomingBinaryMessages.Dequeue();
                    OnBinaryMessageReceived?.Invoke(data);
                }
            }
        }

        private static string BytesToHexString(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "[]";

            StringBuilder sb = new("[");
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2")); // "X2" formats byte as two-digit hexadecimal
                if (i < bytes.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
