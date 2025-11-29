using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using CosmicScavengers.Networking.Meta;
using CosmicScavengers.Networking.Event.Channels;

namespace CosmicScavengers.Networking
{
    /// <summary>
    /// Handles the low-level TCP connection using a multiplexed protocol: 
    /// a 4-byte length prefix, followed by a 1-byte type header and the payload.
    /// This component ONLY routes data and has NO application logic (e.g., login, game rules).
    /// </summary>
    public class ClientConnector : MonoBehaviour
    {
        // --- Server Connection Details ---
        private const string HOST = "127.0.0.1";
        private const int PORT = 8080;

        private TcpClient client;
        private NetworkStream stream;
        private Thread clientThread;

        // Thread-safe queues to hold messages received from the server
        private readonly Queue<string> incomingTextMessages = new();
        private readonly Queue<byte[]> incomingBinaryMessages = new();

        // Events for consumers (Auth, GameState) to subscribe to
        public event Action<string> OnTextMessageReceived;
        public event Action<byte[]> OnBinaryMessageReceived;

        public delegate void ConnectionEstablishedHandler();
        public event ConnectionEstablishedHandler OnConnected;

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        void Start()
        {
            Debug.Log("[Connector] Attempting to connect to server...");
            clientThread = new Thread(ConnectToServer)
            {
                IsBackground = true
            };
            clientThread.Start();
        }

        public void InitHandshake()
        {
            Debug.Log("[Connector] Initiating handshake with server...");
            SendTextMessage("C_CONNECT");
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
                    Debug.LogError("[Connector Error] Unable to connect to server.");
                    return;
                }
                Debug.Log("[Connector] Successfully connected to the multiplexed server!");

                OnConnected?.Invoke();

                // Start listening loop                
                while (client.Connected)
                {
                    ReadNextMessage();
                }
            }
            catch (SocketException e)
            {
                Debug.LogError($"[Connector Error] Connection failed (Socket Exception): {e.Message}. Is the server running on {HOST}:{PORT}?");
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
                Debug.LogError("[Connector Error] Stream is not readable.");
                return;
            }

            const int LENGTH_FIELD_SIZE = 4;
            byte[] lengthBytes = new byte[LENGTH_FIELD_SIZE];

            try
            {
                // Read the 4-byte length prefix
                if (ReadExactly(stream, lengthBytes, 0, LENGTH_FIELD_SIZE) == 0)
                {
                    Debug.LogWarning("[Connector] Stream closed by server.");
                    return;
                }

                // Convert Big-Endian (Netty) to local machine Endian
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }

                int messageLength = BitConverter.ToInt32(lengthBytes, 0);
                if (messageLength <= 0 || messageLength > 1024 * 1024)
                {
                    Debug.LogError($"[Connector Error] Invalid message length received: {messageLength}.");
                    client.Close();
                    return;
                }

                // Read Payload (L bytes, including the 1-byte Type prefix)
                byte[] fullPayloadBytes = new byte[messageLength];
                ReadExactly(stream, fullPayloadBytes, 0, messageLength);
                // Extract Type and Payload Data
                MessageType messageType = (MessageType)fullPayloadBytes[0];

                int payloadDataLength = messageLength - 1;
                if (payloadDataLength < 0)
                {
                    Debug.LogError("[Connector Error] Payload data length is negative. Discarding message.");
                    return;
                }

                byte[] payloadData = new byte[payloadDataLength];
                Array.Copy(fullPayloadBytes, 1, payloadData, 0, payloadDataLength);

                if (messageType == MessageType.TEXT)
                {
                    string message = Encoding.UTF8.GetString(payloadData).Trim();
                    lock (incomingTextMessages)
                    {
                        incomingTextMessages.Enqueue(message);
                    }
                }
                else if (messageType == MessageType.BINARY)
                {
                    lock (incomingBinaryMessages)
                    {
                        incomingBinaryMessages.Enqueue(payloadData);
                    }
                }
                else
                {
                    Debug.LogWarning($"[Connector] Received unknown message type: {messageType}. Discarding payload.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Connector Error] Reading failed: {ex.Message}");
                Cleanup();
            }
        }

        private int ReadExactly(NetworkStream targetStream, byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = targetStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    Debug.LogWarning("[Connector] Stream closed by server.");
                    return 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// Sends a text command, automatically framing it with MessageType.TEXT.
        /// Used by ClientAuth/Lobby Managers.
        /// </summary>
        public virtual void SendTextMessage(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                Debug.LogWarning("Cannot send empty command.");
                return;
            }
            SendTypedMessage(Encoding.UTF8.GetBytes(text), MessageType.TEXT);
        }

        /// <summary>
        /// Sends raw binary data, automatically framing it with MessageType.BINARY.
        /// Used by ClientGameState for high-frequency updates.
        /// </summary>
        public virtual void SendBinaryMessage(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning("Cannot send empty binary data.");
                return;
            }
            SendTypedMessage(data, MessageType.BINARY);
        }

        /// <summary>
        /// Core method for sending messages, constructing the full frame:
        /// [4-byte Length] + [1-byte Type] + [Payload]
        /// </summary>
        private void SendTypedMessage(byte[] dataBytes, MessageType type)
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
                lock (stream)
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
                    OnBinaryMessageReceived?.Invoke(incomingBinaryMessages.Dequeue());
                }
            }
        }
    }
}