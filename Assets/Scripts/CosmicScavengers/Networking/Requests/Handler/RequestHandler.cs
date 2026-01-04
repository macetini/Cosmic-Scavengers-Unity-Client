using System;
using System.Collections.Generic;
using CosmicScavengers.Networking.Channel;
using CosmicScavengers.Networking.Commands;
using CosmicScavengers.Networking.Commands.Data.Binary;
using CosmicScavengers.Networking.Commands.Data.Meta;
using CosmicScavengers.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Request.Binary.Data;
using CosmicScavengers.Networking.Request.Registry;
using CosmicScavengers.Networking.Request.Text.Data;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Handler
{
    public class RequestHandler : MonoBehaviour
    {
        [Header("Request Channels")]
        [SerializeField]
        [Tooltip("Channel for Requests.")]
        private CommandChannel requestCommandChannel;

        [Header("Registry Configuration")]
        [SerializeField]
        [Tooltip("Registry containing binary request handlers.")]
        private BinaryRequestRegistry binaryRequestRegistry;

        [SerializeField]
        [Tooltip("Registry containing text request handlers.")]
        private TextRequestRegistry textRequestRegistry;

        [Header("Binary Requests Container")]
        [SerializeField]
        [Tooltip("Container for instantiated binary requests.")]
        private GameObject binaryRequestsContainer;

        [SerializeField]
        [Tooltip("Container for instantiated text requests.")]
        private GameObject textRequestsContainer;

        private readonly Dictionary<NetworkBinaryCommand, BaseBinaryRequest> requestLookup = new();

        void Awake()
        {
            if (requestCommandChannel == null)
            {
                Debug.LogError("RequestCommandChannel is not assigned in RequestHandlers.");
            }

            if (binaryRequestRegistry == null)
            {
                Debug.LogError("BinaryRequestRegistry is not assigned in RequestHandlers.");
            }
            if (textRequestRegistry == null)
            {
                Debug.LogError("TextRequestRegistry is not assigned in RequestHandlers.");
            }

            if (binaryRequestsContainer == null)
            {
                Debug.LogWarning(
                    "RequestsContainer is not assigned. Creating a new GameObject for it."
                );
                binaryRequestsContainer = Instantiate(
                    new GameObject("BinaryRequestsContainer"),
                    transform
                );
            }
            if (textRequestsContainer == null)
            {
                Debug.LogWarning(
                    "TextRequestsContainer is not assigned. Creating a new GameObject for it."
                );
                textRequestsContainer = Instantiate(
                    new GameObject("TextRequestsContainer"),
                    transform
                );
            }
        }

        void OnEnable()
        {
            //requestCommandChannel.AddListener(HandleRequestCommand);
        }

        void OnDisable()
        {
            //requestCommandChannel.RemoveListener(HandleRequestCommand);
        }

        private void HandleRequestCommand(BaseNetworkCommand command, object data)
        {
            Debug.Log($"[RequestHandlers] Handling Request Command with Command ID: {command}");

            switch (command.Type)
            {
                case CommandType.BINARY:
                    HandleBinaryRequest(command.BinaryCommand, data);
                    break;
                case CommandType.TEXT:
                    //HandleTextRequest(command.TextCommand, data);
                    break;
                case CommandType.UNKNOWN:
                    Debug.LogWarning(
                        $"[RequestHandlers] - Received Unknown Command Type: {command.Type}"
                    );
                    break;
                default:
                    throw new Exception($"Impossible Command Type: {command.Type}");
            }
        }

        private void HandleBinaryRequest(NetworkBinaryCommand command, object data)
        {
            Debug.Log($"[RequestHandlers] Handling Binary Request with Command ID: {command}");

            if (requestLookup.TryGetValue(command, out var existingRequest))
            {
                //existingRequest.Execute(data);
                return;
            }

            BaseBinaryRequest requestPrefab = binaryRequestRegistry.GetPrefab(command);
            if (requestPrefab == null)
            {
                Debug.LogWarning(
                    $"[RequestHandlers] No request prefab found for Command ID: {command}"
                );
                return;
            }
            var requestInstance = Instantiate(requestPrefab, binaryRequestsContainer.transform);
            requestLookup[command] = requestInstance;
            //requestInstance.Execute(data);
        }

        private void HandleTextRequest(NetworkTextCommand command, string data)
        {
            Debug.Log($"[RequestHandlers] Handling Text Request with Command ID: {command}");

            BaseTextRequest requestPrefab = textRequestRegistry.GetPrefab(command);
            if (requestPrefab == null)
            {
                Debug.LogWarning(
                    $"[RequestHandlers] No request prefab found for Command ID: {command}"
                );
                return;
            }
            var requestInstance = Instantiate(requestPrefab, textRequestsContainer.transform);
            //requestLookup[command] = requestInstance;
            //requestInstance.Execute(data);
        }
    }
}
