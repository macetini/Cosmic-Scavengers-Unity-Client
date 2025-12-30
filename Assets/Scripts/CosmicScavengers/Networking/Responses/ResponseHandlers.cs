using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Response.Registry;
using CosmicScavengers.Core.Networking.Responses.Channels;
using CosmicScavengers.Core.Networking.Responses.Data;
using UnityEngine;

public class ResponseHandlers : MonoBehaviour
{
    [Header("Channel Configuration")]
    [SerializeField]
    [Tooltip("Channel to listen for binary responses.")]
    BinaryResponseChannel binaryResponseChannel;

    [Header("Registry Configuration")]
    [SerializeField]
    [Tooltip("Registry containing binary response handlers.")]
    private BinaryResponseRegistry binaryResponseRegistry;

    [SerializeField]
    [Tooltip("Channel to listen for response handlers")]
    private GameObject responsesContainer;

    private readonly Dictionary<NetworkBinaryCommand, BaseBinaryResponse> responseLookup = new();

    void Awake()
    {
        if (binaryResponseChannel == null)
        {
            Debug.LogError("BinaryResponseChannel is not assigned in ResponseHandlers.");
        }
        if (binaryResponseRegistry == null)
        {
            Debug.LogError("BinaryResponseRegistry is not assigned in ResponseHandlers.");
        }
        if (responsesContainer == null)
        {
            Debug.LogWarning(
                "ResponsesContainer is not assigned. Creating a new GameObject for it."
            );
            responsesContainer = Instantiate(new GameObject("ResponsesContainer"), transform);
        }
    }

    void OnEnable()
    {
        binaryResponseChannel.AddListener(HandleBinaryResponse);
    }

    void OnDisable()
    {
        binaryResponseChannel.RemoveListener(HandleBinaryResponse);
    }

    private void HandleBinaryResponse(NetworkBinaryCommand command, byte[] data)
    {
        Debug.Log($"[ResponseHandlers] Handling Binary Response with Command ID: {command}");
        if (responseLookup.TryGetValue(command, out var existingResponse))
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
        var responseInstance = Instantiate(responsePrefab, responsesContainer.transform);
        responseLookup[command] = responseInstance;

        responseInstance.Handle(data);
    }
}
