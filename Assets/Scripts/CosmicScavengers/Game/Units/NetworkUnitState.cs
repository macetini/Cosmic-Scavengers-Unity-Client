using Assets.Scripts.CosmicScavengers.Networking;
using UnityEngine;

namespace Assets.Scripts.CosmicScavengers.Game.Units
{
    public class NetworkUnitState : MonoBehaviour
    {
        // The final, authoritative position last received from the server
        private Vector3 serverPosition;

        // --- Editor Field ---
        // Adjust this factor to control how fast the client's visual position catches up
        [SerializeField] private float smoothingFactor = 10f;

        // --- Identity ---
        public int UnitID { get; private set; } // Matches the ID from the Java server

        void Start()
        {
            // Initialize positions to the current transform to prevent any initial snap
            serverPosition = transform.position;
        }

        void Update()
        {
            // This is the core visualization loop. It smoothly moves the object
            // from its current visual position toward the authoritative server position.
            transform.position = Vector3.Lerp(
                transform.position,
                serverPosition,
                Time.deltaTime * smoothingFactor
            );
        }

        // --- Core Network Handler ---

        /**
         * Called by GameWorldManager whenever a U_POS message for this unit is received.
         * This updates the authoritative target position for the Lerp function.
         */
        public void ReceiveServerUpdate(UnitStateData data)
        {
            // 1. Update the authoritative position based on the server data
            // Assuming your game world uses 2D (X, Y) and Z is zero.
            serverPosition = new Vector3(data.PositionX, data.PositionY, 0);

            // 2. Future: You would handle rotation updates (for unit facing) or animation triggers here.
            // Example: health = data.Health;
        }

        // --- Initialization ---

        /**
         * Called by GameWorldManager when a U_CREATE message is received to spawn this unit.
         */
        public void Initialize(int id, Vector3 initialPos)
        {
            UnitID = id;
            transform.position = initialPos;
            serverPosition = initialPos; // Initialize the target position
            gameObject.name = "Unit_" + id;
        }
    }
}