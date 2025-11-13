using UnityEngine;
using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace Assets.Scripts.CosmicScavengers.Networking
{
    /// <summary>
    /// Handles the low-level TCP connection, threading, and raw message passing.
    /// It exposes a public event for higher-level managers (like ClientAuth or GameState) 
    /// to handle protocol logic based on the user's authentication status.
    /// </summary>
    public class ClientConnector : MonoBehaviour
    {
        // --- Server Connection Details ---
        private const string HOST = "127.0.0.1";
        private const int PORT = 8080;
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private Thread clientThread;

        // Thread-safe queue to hold messages received from the server
        private readonly Queue<string> incomingMessages = new();

        // PUBLIC EVENT: External classes subscribe to this to receive messages
        public event Action<string> OnMessageReceived;

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        void Start()
        {
            Debug.Log("Attempting to connect to server...");
            clientThread = new Thread(ConnectToServer)
            {
                IsBackground = true
            };
            clientThread.Start();
        }

        void ConnectToServer()
        {
            try
            {
                client = new TcpClient(HOST, PORT);
                stream = client.GetStream();
                writer = new StreamWriter(stream) { AutoFlush = true };
                StreamReader reader = new(stream);

                Debug.Log("Successfully connected to the Java server!");

                // Start listening loop
                while (client.Connected)
                {
                    string message = reader.ReadLine();
                    if (message != null)
                    {
                        // Enqueue the raw message to be processed in the main Unity thread
                        lock (incomingMessages)
                        {
                            incomingMessages.Enqueue(message.Trim());
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.LogError($"Connection failed (Socket Exception): {e.Message}. Is the server running on {HOST}:{PORT}?");
            }
            catch (ThreadAbortException e)
            {
                Debug.Log($"Client thread aborted: {e.Message}");
            }
            catch (IOException e)
            {
                Debug.LogError($"Connection lost: {e.Message}");
            }
            finally
            {
                stream?.Close();
                client?.Close();
                Debug.Log("Disconnected or connection attempt failed. Cleanup complete.");
            }
        }

        /// <summary>
        /// Public method to send a raw protocol command to the server.
        /// </summary>
        public void SendInput(string command)
        {
            if (IsConnected && writer != null)
            {
                writer.WriteLine(command);
                writer.Flush();
                Debug.Log($"[Client] Sent: {command}");
            }
            else
            {
                Debug.LogError("[Client] Cannot send command: Not connected to server.");
            }
        }

        void OnApplicationQuit()
        {
            if (clientThread != null && clientThread.IsAlive)
            {
                clientThread.Abort();
            }
            stream?.Close();
            client?.Close();
        }

        void Update()
        {
            // Process messages received in the background thread on the main Unity thread
            lock (incomingMessages)
            {
                while (incomingMessages.Count > 0)
                {
                    string rawMessage = incomingMessages.Dequeue();
                    
                    // Fire event for ALL listeners. Authentication, Game State, etc., will handle it.
                    OnMessageReceived?.Invoke(rawMessage);
                    
                    // Log the raw message here so it's always visible in the console
                    Debug.Log($"[RAW SERVER MESSAGE]: {rawMessage}"); 
                }
            }
        }
    }
}