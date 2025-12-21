using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using CosmicScavengers.Core.Networking.Commands.Meta;
using UnityEngine;

namespace CosmicScavengers.Core.Networking
{
    /// <summary>
    /// Manages low-level TCP connection to the multiplexed server.
    /// Handles sending and receiving of both text and binary messages.
    /// </summary>
    public class ClientConnector : MonoBehaviour
    {
        // --- Server Connection Details ---
        private const string HOST = "127.0.0.1";
        private const int PORT = 8080;
        private const int LENGTH_FIELD_SIZE = 4; // 4 bytes for length prefix
        private const int MAX_MESSAGE_SIZE = 1024 * 1024; // 1 MB

        // -----------------------------------

        // Networking components
        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;

        // Lock object for thread-safe stream operations
        private readonly object streamLock = new();

        // Thread-safe queues to hold messages received from the server
        private readonly Queue<string> incomingTextMessages = new();
        private readonly Queue<byte[]> incomingBinaryMessages = new();

        /// Events for incoming messages
        public event Action<string> OnTextMessageReceived;
        public event Action<byte[]> OnBinaryMessageReceived;

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

                InitHandshake();

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
        /// Initiates the handshake process with the server by sending a connect command.
        /// </summary>
        public void InitHandshake()
        {
            Debug.Log("[Connector] Initiating handshake with server.");
            DispatchTextMessage("C_CONNECT");
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
        /// Sends a text command, automatically framing it with MessageType.TEXT.
        /// Used by ClientAuth/Lobby Managers.
        /// </summary>
        public void DispatchTextMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("Cannot send empty command.");
                return;
            }
            SendTypedMessage(Encoding.UTF8.GetBytes(text), CommandType.TEXT);
        }

        /// <summary>
        /// Sends raw binary data, automatically framing it with MessageType.BINARY.
        /// Used by ClientGameState for high-frequency updates.
        /// </summary>
        public void DispatchBinaryMessage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning("Cannot send empty binary data.");
                return;
            }
            SendTypedMessage(data, CommandType.BINARY);
        }

        /// <summary>
        /// Core method for sending messages, constructing the full frame:
        /// [4-byte Length] + [1-byte Type] + [Payload]
        /// </summary>
        private void SendTypedMessage(byte[] dataBytes, CommandType type)
        {
            if (!IsConnected || stream == null)
            {
                Debug.LogError($"[Connector] Cannot send {type} message: Not connected to server.");
                return;
            }
            Debug.Log($"[Connector] Sending {type} message of length {dataBytes.Length} bytes.");

            try
            {
                // Payload length = 1 (Type byte) + Data length
                int payloadLength = 1 + dataBytes.Length;
                byte[] lengthBytes = BitConverter.GetBytes(payloadLength);

                // Convert to Big-Endian for Java/Netty compatibility
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                using MemoryStream ms = new();
                // 1. Write 4-byte Length Prefix (Big-Endian)
                ms.Write(lengthBytes, 0, lengthBytes.Length);
                // 2. Write 1-byte Message Type
                ms.WriteByte((byte)type);
                // 3. Write the Payload Data
                ms.Write(dataBytes, 0, dataBytes.Length);

                byte[] finalBuffer = ms.ToArray();
                lock (streamLock)
                {
                    stream.Write(finalBuffer, 0, finalBuffer.Length);
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
            // Process text messages (for Auth/Commands)
            lock (incomingTextMessages)
            {
                while (incomingTextMessages.Count > 0)
                {
                    OnTextMessageReceived?.Invoke(incomingTextMessages.Dequeue());
                }
            }

            // Process binary messages (for Game State)
            lock (incomingBinaryMessages)
            {
                while (incomingBinaryMessages.Count > 0)
                {
                    byte[] data = incomingBinaryMessages.Dequeue();

                    // --- DEBUG LOGGING ---
                    // Debug.Log($"[Connector Debug] Received Binary Message ({data.Length} bytes): {BytesToHexString(data)}");
                    // ---------------------

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
