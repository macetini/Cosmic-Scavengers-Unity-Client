using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands.Channel.Inbound;
using CosmicScavengers.Core.Networking.Commands.Data;
using CosmicScavengers.Core.Networking.Commands.Data.Binary;
using CosmicScavengers.Core.Networking.Commands.Data.Meta;
using CosmicScavengers.Core.Networking.Commands.Data.Text;
using CosmicScavengers.Networking.Commands.Responses.Data.Binary;
using CosmicScavengers.Networking.Commands.Responses.Data.Text;
using CosmicScavengers.Networking.Commands.Responses.Registry;
using CosmicScavengers.Networking.Response.Registry;
using UnityEngine;

namespace CosmicScavengers.Networking.Commands.Responses.Handler
{
    public class ResponseHandler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Channel to listen for inbound networking message handlers.")]
        NetworkingInboundChannel inboundChannel;

        [Header("Registry Configuration")]
        [SerializeField]
        [Tooltip("Registry containing binary response handlers.")]
        private BinaryResponseRegistry binaryResponseRegistry;

        [SerializeField]
        [Tooltip("Registry containing text response handlers.")]
        private TextResponseRegistry textResponseRegistry;

        [Header("Response Containers")]
        [SerializeField]
        [Tooltip("GameObject Container for binary response handlers")]
        private GameObject binaryHandlersContainer;

        [SerializeField]
        [Tooltip("GameObject Container for text response handlers")]
        private GameObject textHandlersContainer;

        private readonly Dictionary<
            NetworkBinaryCommand,
            BaseBinaryResponse
        > binaryResponsesLookup = new();

        private readonly Dictionary<NetworkTextCommand, BaseTextResponse> textResponsesLookup =
            new();

        void Awake()
        {
            if (inboundChannel == null)
            {
                Debug.LogError("ResponseChannel is not assigned in ResponseHandlers.");
            }

            if (binaryResponseRegistry == null)
            {
                Debug.LogError("BinaryResponseRegistry is not assigned in ResponseHandlers.");
            }
            if (textResponseRegistry == null)
            {
                Debug.LogError("TextResponseRegistry is not assigned in ResponseHandlers.");
            }

            if (binaryHandlersContainer == null)
            {
                Debug.LogWarning(
                    "ResponsesContainer is not assigned. Creating a new GameObject for it."
                );
                binaryHandlersContainer = Instantiate(new GameObject("BinaryHandlers"), transform);
            }
            if (textHandlersContainer == null)
            {
                Debug.LogWarning(
                    "ResponsesContainer is not assigned. Creating a new GameObject for it."
                );
                textHandlersContainer = Instantiate(new GameObject("TextHandlers"), transform);
            }
        }

        void OnEnable()
        {
            inboundChannel.AddListener(RouteResponse);
        }

        void OnDisable()
        {
            inboundChannel.RemoveListener(RouteResponse);
        }

        private void RouteResponse(BaseNetworkCommand command, InboundData responseData)
        {
            switch (command.Type)
            {
                case CommandType.BINARY:
                    HandleBinaryResponse(command.BinaryCommand, responseData.RawBytes);
                    break;
                case CommandType.TEXT:
                    HandleBinaryResponse(command.TextCommand, responseData.TextParts);
                    break;
                case CommandType.UNKNOWN:
                    Debug.LogWarning(
                        $"[ResponseHandlers] - Received Unknown Command Type: {command.Type}"
                    );
                    break;
                default:
                    throw new Exception($"Impossible Command Type: {command.Type}");
            }
        }

        private void HandleBinaryResponse(NetworkBinaryCommand command, byte[] data)
        {
            Debug.Log($"[ResponseHandlers] Handling Binary Response with Command ID: {command}");
            if (binaryResponsesLookup.TryGetValue(command, out var existingResponse))
            {
                existingResponse.Handle(data);
                return;
            }

            BaseBinaryResponse responsePrefab = binaryResponseRegistry.GetPrefab(command);
            if (responsePrefab == null)
            {
                Debug.LogWarning(
                    $"[ResponseHandlers] No response prefab found for Command ID: {command}"
                );
                return;
            }
            var responseInstance = Instantiate(responsePrefab, binaryHandlersContainer.transform);
            binaryResponsesLookup[command] = responseInstance;

            responseInstance.Handle(data);
        }

        private void HandleBinaryResponse(NetworkTextCommand command, string[] data)
        {
            Debug.Log($"[ResponseHandlers] Handling Text Response with Command ID: {command}");
            if (textResponsesLookup.TryGetValue(command, out var existingResponse))
            {
                existingResponse.Handle(data);
                return;
            }

            BaseTextResponse responsePrefab = textResponseRegistry.GetPrefab(command);
            if (responsePrefab == null)
            {
                Debug.LogWarning(
                    $"[ResponseHandlers] No response prefab found for Command ID: {command}"
                );
                return;
            }
            var responseInstance = Instantiate(responsePrefab, textHandlersContainer.transform);
            textResponsesLookup[command] = responseInstance;

            responseInstance.Handle(data);
        }
    }
}
