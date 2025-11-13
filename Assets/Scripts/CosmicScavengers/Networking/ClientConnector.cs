using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Assets.Scripts.CosmicScavengers.Game; // For the message queue

namespace Assets.Scripts.CosmicScavengers.Networking
{
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

        public bool IsConnected
        {
            get { return client != null && client.Connected; }
        }

        void Start()
        {
            Debug.Log("Attempting to connect to server...");
            // Start the connection process in a separate thread
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

                // 2. Start listening loop
                while (client.Connected)
                {
                    string message = reader.ReadLine();
                    if (message != null)
                    {
                        lock (incomingMessages)
                        {
                            incomingMessages.Enqueue(message);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                // This is expected if the server is off or the IP is wrong
                Debug.LogError($"Connection failed (Socket Exception): {e.Message}");
            }
            catch (ThreadAbortException e)
            {
                // Thread was aborted, likely due to application quitting
                Debug.Log($"Client thread aborted: {e.Message}");
            }
            finally
            {
                // 3. Cleanup on disconnect
                stream?.Close();
                client?.Close();
            }
        }

        // Public method to send player input (e.g., a test message or a move command)
        public void SendInput(string command)
        {
            // Ensure the connection is open before writing
            if (IsConnected && writer != null)
            {
                // Netty server expects a newline delimiter
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
            // Cleanly stop the background thread and close the connection
            clientThread?.Abort();
            stream?.Close();
            client?.Close();
        }

        void Update()
        {
            // Process messages received in the background thread on the main Unity thread
            lock (incomingMessages) // Ensure thread safety when accessing the queue
            {
                while (incomingMessages.Count > 0)
                {
                    string rawMessage = incomingMessages.Dequeue();
                    HandleServerMessage(rawMessage);
                }
            }
        }

        private static void HandleServerMessage(string rawMessage)
        {
            if (string.IsNullOrEmpty(rawMessage)) return;
            string commandCode = rawMessage.Split('|')[0];

            // --- Authentication Response Handling ---
            if (commandCode == "SUCCESS" || commandCode == "ERROR")
            {
                Debug.Log($"[SERVER RESPONSE]: {rawMessage}");
                return;
            }

            if (commandCode == "U_POS")
            {
                try
                {
                    UnitStateData data = new(rawMessage);
                    // Routes data to the world manager for visualization
                    GameWorldManager.Instance.HandleUnitPositionUpdate(data);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Protocol Error parsing U_POS: {e.Message}");
                }
            }
            else if (commandCode == "U_CREATE")
            {
                // U_CREATE|ID:123|Type:Fighter|Owner:2|X:50.0|Y:50.0
                try
                {
                    string[] parts = rawMessage.Split('|');
                    int id = int.Parse(parts[1].Split(':')[1]);
                    float x = float.Parse(parts[4].Split(':')[1]);
                    float y = float.Parse(parts[5].Split(':')[1]);

                    // Instantiates the unit visual
                    GameWorldManager.Instance.CreateUnit(id, new Vector3(x, y, 0));
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Protocol Error parsing U_CREATE: {e.Message}");
                }
            }
            else
            {
                // Handle chat, status, or other miscellaneous broadcasts
                Debug.Log($"[SERVER BROADCAST]: {rawMessage}");
            }
        }
    }
}