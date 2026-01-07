using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Commands.Data.Meta;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Request.Data.Text;
using CosmicScavengers.Core.Networking.Commands.Request.Registry;
using CosmicScavengers.Core.Networking.Commands.Requests.Channel;
using CosmicScavengers.Core.Networking.Commands.Requests.Data;
using CosmicScavengers.Core.Networking.Request.Data.Binary;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Commands.Requests.Handler
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

        private readonly Dictionary<BaseNetworkCommand, BaseRequest<object>> requestsLookup = new();

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

        private void RouteRequest(BaseNetworkCommand command, object[] data)
        {
            switch (command.Type)
            {
                case CommandType.TEXT:
                    HandleTextRequest(command.TextCommand, data);
                    break;
                case CommandType.BINARY:
                    HandleBinaryRequest(command.BinaryCommand, data);
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

        private void HandleTextRequest(NetworkTextCommand command, object[] data)
        {
            Debug.Log($"[RequestHandlers] Handling Text Request with Command ID: {command}");

            if (requestsLookup.TryGetValue(command, out var existingRequest))
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
            requestsLookup[command] = requestInstance;
            requestInstance.Execute(data);
        }

        private void HandleBinaryRequest(NetworkBinaryCommand command, object[] data)
        {
            Debug.Log($"[RequestHandlers] Handling Binary Request with Command ID: {command}");

            if (requestsLookup.TryGetValue(command, out var existingRequest))
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
            requestsLookup[command] = requestInstance;
            requestInstance.Execute(data);
        }
    }
}
