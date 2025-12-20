using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CosmicScavengers.Networking.Requests;
using UnityEngine;

namespace CosmicScavengers.Networking.Communication
{
    /// <summary>
    /// Discovers all INetworkRequest components in the scene and provides
    /// a dynamic way to trigger them.
    /// </summary>
    public class NetworkRequestDispatcher : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private ClientConnector clientConnector;

        [Header("Debug")]
        [Tooltip("Visual list of registered requests (Read Only)")]
        [SerializeField, ReadOnly(true)]
        private List<string> registeredRequestsCode = new();

        private readonly Dictionary<short, INetworkRequest> requestsByCode = new();

        [ContextMenu("Assign Requests (Preview)")]
        private void AssignRequestsPreview()
        {
            PopulateRequestDictionary(true);
        }

        private void PopulateRequestDictionary(bool isPreview = false)
        {
            requestsByCode.Clear();
            registeredRequestsCode.Clear();

            // Find all MonoBehaviours that implement INetworkRequest
            var found = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<INetworkRequest>();

            foreach (var req in found)
            {
                short commandCode = (short)req.CommandCode;
                if (!requestsByCode.ContainsKey(commandCode))
                {
                    requestsByCode.Add(commandCode, req);
                    registeredRequestsCode.Add($"{commandCode}: {req.GetType().Name}");
                }
                else
                {
                    Debug.LogWarning(
                        $"[NetworkRequestDispatcher] Duplicate request code {commandCode} found in {req.GetType().Name}!"
                    );
                }
            }

            if (isPreview)
            {
                Debug.Log(
                    $"[NetworkRequestDispatcher] Registered {requestsByCode.Count} dynamic requests."
                );
            }
        }

        public void Dispatch(params object[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                Debug.LogWarning("[NetworkRequestDispatcher] No parameters provided for dispatch.");
                return;
            }

            short commandCode = (short)parameters[0];
            if (requestsByCode.TryGetValue(commandCode, out var request))
            {
                request.Dispatch(clientConnector, parameters.Skip(1).ToArray());
            }
            else
            {
                Debug.LogWarning(
                    $"[NetworkRequestDispatcher] No request found for command code {commandCode}."
                );
            }
        }
    }
}
