using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.CosmicScavengers.Game.Units;
using Assets.Scripts.CosmicScavengers.Networking;

namespace Assets.Scripts.CosmicScavengers.Game
{
    public class GameWorldManager : MonoBehaviour
    {
        // --- Singleton Setup ---
        public static GameWorldManager Instance { get; private set; }

        // --- Editor Fields ---
        [SerializeField]
        private GameObject unitPrefab;

        // --- Game State ---
        // Map to track all active unit components by their unique Server ID
        private readonly Dictionary<int, NetworkUnitState> activeUnits = new();

        void Awake()
        {
            // Simple Singleton initialization for easy access
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // --- 1. HANDLE POSITION UPDATES (U_POS) ---

        /**
         * Called by ClientConnector when a U_POS message is received.
         * This routes the server data to the correct unit for visualization.
         */
        public void HandleUnitPositionUpdate(UnitStateData data)
        {
            if (activeUnits.TryGetValue(data.UnitID, out NetworkUnitState unitState))
            {
                // Unit exists: Pass the parsed data to the unit's component for interpolation
                unitState.ReceiveServerUpdate(data);
            }
            else
            {
                // Should only happen if U_POS arrives before U_CREATE, or if the unit is unknown
                Debug.LogWarning($"Received position update for unknown Unit ID: {data.UnitID}.");
            }
        }

        // --- 2. HANDLE UNIT CREATION (U_CREATE) ---

        /**
         * Called by ClientConnector when a U_CREATE message is received.
         * This instantiates the visual representation of the unit in the game world.
         * @param id The server-assigned unique ID.
         * @param pos The initial world position.
         */
        public void CreateUnit(int id, Vector3 pos)
        {
            if (unitPrefab == null)
            {
                Debug.LogError("Unit Prefab is not assigned in the Inspector!");
                return;
            }

            if (!activeUnits.ContainsKey(id))
            {
                // Instantiate the unit and get its component
                GameObject newUnitObject = Instantiate(unitPrefab, pos, Quaternion.identity);

                if (newUnitObject.TryGetComponent<NetworkUnitState>(out var unitState))
                {
                    // Initialize the unit's component with its unique ID
                    Debug.Log($"Creating unit ID {id} at position {pos}.");
                    unitState.Initialize(id, pos);
                    activeUnits.Add(id, unitState);
                }
                else
                {
                    Debug.LogError($"Unit Prefab is missing the {nameof(NetworkUnitState)} component!");
                    Destroy(newUnitObject);
                }
            }
        }

        // --- 3. HANDLE UNIT DELETION (U_DEL) ---

        /**
         * Future method: Called when a U_DEL message is received (e.g., unit destroyed).
         */
        public void DestroyUnit(int id)
        {
            if (activeUnits.ContainsKey(id))
            {
                Destroy(activeUnits[id].gameObject);
                activeUnits.Remove(id);
                Debug.Log($"Destroyed unit ID {id}.");
            }
        }
    }
}