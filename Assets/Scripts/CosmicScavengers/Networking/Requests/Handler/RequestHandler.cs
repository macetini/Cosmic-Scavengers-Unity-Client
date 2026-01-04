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
using CosmicScavengers.Networking.Requests.Channel;
using UnityEngine;

namespace CosmicScavengers.Networking.Requests.Handler
{
    public class RequestHandler : MonoBehaviour
    {
        [Header("Request Channels")]
        [SerializeField]
        [Tooltip("Channel for Requests.")]
        private RequestChannel requestChannel;

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

        private readonly Dictionary<BaseNetworkCommand, BaseBinaryRequest> binaryRequestsLookup =
            new();
        private readonly Dictionary<BaseNetworkCommand, BaseTextRequest> textRequestsLookup = new();

        void Awake()
        {
            if (requestChannel == null)
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
            requestChannel.AddListener(RouteRequest);
        }

        void OnDisable()
        {
            requestChannel.RemoveListener(RouteRequest);
        }

        private void RouteRequest(BaseNetworkCommand command, RequestChannelData data)
        {
            switch (command.Type)
            {
                case CommandType.BINARY:
                    HandleBinaryRequest(command.BinaryCommand, data.ObjectParts);
                    break;
                case CommandType.TEXT:
                    HandleTextRequest(command.TextCommand, data.TextParts);
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

        private void HandleBinaryRequest(NetworkBinaryCommand command, object[] data)
        {
            Debug.Log($"[RequestHandlers] Handling Binary Request with Command ID: {command}");

            if (binaryRequestsLookup.TryGetValue(command, out var existingRequest))
            {
                existingRequest.Execute(data);
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
            binaryRequestsLookup[command] = requestInstance;
            requestInstance.Execute(data);
        }

        private void HandleTextRequest(NetworkTextCommand command, string[] data)
        {
            Debug.Log($"[RequestHandlers] Handling Text Request with Command ID: {command}");

            if (textRequestsLookup.TryGetValue(command, out var existingRequest))
            {
                existingRequest.Execute(data);
                return;
            }

            BaseTextRequest requestPrefab = textRequestRegistry.GetPrefab(command);
            if (requestPrefab == null)
            {
                Debug.LogWarning(
                    $"[RequestHandlers] No request prefab found for Command ID: {command}"
                );
                return;
            }
            var requestInstance = Instantiate(requestPrefab, textRequestsContainer.transform);
            textRequestsLookup[command] = requestInstance;
            requestInstance.Execute(data);
        }
    }
}
