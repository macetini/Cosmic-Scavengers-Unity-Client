using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data;
using CosmicScavengers.Core.Networking.Request.Registry;
using CosmicScavengers.Core.Networking.Requests.Channels;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Requests
{
    public class RequestHandlers : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to listen for binary requests.")]
        BinaryRequestChannel binaryRequestChannel;

        [SerializeField]
        [Tooltip("Channel to listen for text requests.")]
        TextRequestChannel textRequestChannel;

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
            if (binaryRequestChannel == null)
            {
                Debug.LogError("BinaryRequestChannel is not assigned in RequestHandlers.");
            }
            if (textRequestChannel == null)
            {
                Debug.LogError("TextRequestChannel is not assigned in RequestHandlers.");
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
            binaryRequestChannel.AddListener(HandleBinaryRequest);
            textRequestChannel.AddListener(HandleTextRequest);
        }

        void OnDisable()
        {
            binaryRequestChannel.RemoveListener(HandleBinaryRequest);
            textRequestChannel.RemoveListener(HandleTextRequest);
        }

        private void HandleBinaryRequest(NetworkBinaryCommand command, byte[] data)
        {
            Debug.Log($"[RequestHandlers] Handling Binary Request with Command ID: {command}");

            if (requestLookup.TryGetValue(command, out var existingRequest))
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
            requestLookup[command] = requestInstance;

            requestInstance.Execute(data);
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
            requestInstance.Execute(data);
        }
    }
}
