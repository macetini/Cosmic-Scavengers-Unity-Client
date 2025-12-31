using System;
using System.Collections.Generic;
using CosmicScavengers.Core.Networking.Commands;
using CosmicScavengers.Core.Networking.Response.Registry;
using CosmicScavengers.Core.Networking.Responses.Channels;
using CosmicScavengers.Core.Networking.Responses.Data;
using UnityEngine;
using UnityEngine.UI;

public class ResponseHandlers : MonoBehaviour
{
    [Header("Channel Configuration")]
    [SerializeField]
    [Tooltip("Channel to listen for binary responses.")]
    BinaryResponseChannel binaryResponseChannel;

    [SerializeField]
    [Tooltip("Channel to listen for text responses.")]
    TextResponseChannel textResponseChannel;

    [Header("Registry Configuration")]
    [SerializeField]
    [Tooltip("Registry containing binary response handlers.")]
    private BinaryResponseRegistry binaryResponseRegistry;

    [SerializeField]
    [Tooltip("Registry containing text response handlers.")]
    private TextResponseRegistry textResponseRegistry;

    [Header("Response Containers")]
    [SerializeField]
    [Tooltip("Channel to listen for response handlers")]
    private GameObject binaryResponsesContainer;

    [SerializeField]
    [Tooltip("Channel to listen for response handlers")]
    private GameObject textResponsesContainer;

    private readonly Dictionary<NetworkBinaryCommand, BaseBinaryResponse> binaryResponsesLookup =
        new();
    private readonly Dictionary<NetworkTextCommand, BaseTextResponse> textResponsesLookup = new();

    void Awake()
    {
        if (binaryResponseChannel == null)
        {
            Debug.LogError("BinaryResponseChannel is not assigned in ResponseHandlers.");
        }
        if (textResponseChannel == null)
        {
            Debug.LogError("TextResponseChannel is not assigned in ResponseHandlers.");
        }

        if (binaryResponseRegistry == null)
        {
            Debug.LogError("BinaryResponseRegistry is not assigned in ResponseHandlers.");
        }
        if (textResponseRegistry == null)
        {
            Debug.LogError("TextResponseRegistry is not assigned in ResponseHandlers.");
        }

        if (binaryResponsesContainer == null)
        {
            Debug.LogWarning(
                "ResponsesContainer is not assigned. Creating a new GameObject for it."
            );
            binaryResponsesContainer = Instantiate(
                new GameObject("BinaryResponsesContainer"),
                transform
            );
        }
        if (textResponsesContainer == null)
        {
            Debug.LogWarning(
                "ResponsesContainer is not assigned. Creating a new GameObject for it."
            );
            textResponsesContainer = Instantiate(
                new GameObject("TextResponsesContainer"),
                transform
            );
        }
    }

    void OnEnable()
    {
        binaryResponseChannel.AddListener(HandleBinaryResponse);
        textResponseChannel.AddListener(HandleBinaryResponse);
    }

    void OnDisable()
    {
        binaryResponseChannel.RemoveListener(HandleBinaryResponse);
        textResponseChannel.RemoveListener(HandleBinaryResponse);
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
        var responseInstance = Instantiate(responsePrefab, binaryResponsesContainer.transform);
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
        var responseInstance = Instantiate(responsePrefab, textResponsesContainer.transform);
        textResponsesLookup[command] = responseInstance;

        responseInstance.Handle(data);
    }
}
