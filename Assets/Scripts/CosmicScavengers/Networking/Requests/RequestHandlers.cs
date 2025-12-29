using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Request.Data;
using CosmicScavengers.Core.Networking.Request.Registry;
using UnityEngine;

namespace CosmicScavengers.Core.Networking.Requests
{
    public class RequestHandlers : MonoBehaviour
    {
        [Header("Channel Configuration")]
        [SerializeField]
        [Tooltip("Channel to listen for binary requests.")]
        BinaryRequestChannel binaryRequestChannel;

        [Header("Registry Configuration")]
        [SerializeField]
        [Tooltip("Registry containing binary request handlers.")]
        private BinaryRequestRegistry binaryRequestRegistry;

        [SerializeField]
        [Tooltip("Container for instantiated requests.")]
        private GameObject requestsContainer;

        private Dictionary<NetworkBinaryCommand, BaseBinaryRequest> requestLookup = new();

        void Awake()
        {
            if (binaryRequestChannel == null)
            {
                Debug.LogError("BinaryRequestChannel is not assigned in RequestHandlers.");
            }
            if (binaryRequestRegistry == null)
            {
                Debug.LogError("BinaryRequestRegistry is not assigned in RequestHandlers.");
            }
            if (requestsContainer == null)
            {
                Debug.LogWarning(
                    "RequestsContainer is not assigned. Creating a new GameObject for it."
                );
                requestsContainer = Instantiate(new GameObject("RequestsContainer"), transform);
            }
        }

        void OnEnable()
        {
            binaryRequestChannel.AddListener(HandleBinaryRequest);
        }

        void OnDisable()
        {
            binaryRequestChannel.RemoveListener(HandleBinaryRequest);
        }

        private void HandleBinaryRequest(NetworkBinaryCommand command, object[] args)
        {
            Debug.Log($"[RequestHandlers] Handling Binary Request with Command ID: {command}");

            if (requestLookup.TryGetValue(command, out var existingRequest))
            {
                existingRequest.Execute(args);
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
            var requestInstance = Instantiate(requestPrefab, requestsContainer.transform);
            requestLookup[command] = requestInstance;

            requestInstance.Execute(args);
        }
    }
}
