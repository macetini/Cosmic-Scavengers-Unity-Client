using UnityEngine;
using System; // Required for Exception handling

namespace Assets.Scripts.CosmicScavengers.Networking
{
    // This class represents the data for a single unit's state and position
    public class UnitStateData
    {
        // Data fields matching the protocol
        public int UnitID { get; private set; }
        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public float TargetX { get; private set; }
        public float TargetY { get; private set; } // Added TargetY for completeness
        public int Health { get; private set; } = 100; // Example stat

        private const string COMMAND_CODE = "U_POS";

        // --- CONSTRUCTOR 1: For creating data objects from the server string ---
        public UnitStateData(string rawMessage)
        {
            // Example rawMessage: U_POS|ID:123|X:5.5|Y:10.2|TargetX:11.0

            // 1. Split the entire message by the pipe delimiter '|'
            string[] parts = rawMessage.Split('|');

            // Check for correct command code and minimum parts
            if (parts.Length < 5 || parts[0] != COMMAND_CODE)
            {
                throw new ArgumentException("Invalid or malformed U_POS message received.");
            }

            try
            {
                // 2. Parse the individual fields (we expect KEY:VALUE format)

                // Note: We use int.Parse and float.Parse to convert the string value
                UnitID = int.Parse(parts[1].Split(':')[1]);
                PositionX = float.Parse(parts[2].Split(':')[1]);
                PositionY = float.Parse(parts[3].Split(':')[1]);
                TargetX = float.Parse(parts[4].Split(':')[1]);
                // TargetY will need to be added to the server protocol (TargetX:X|TargetY:Y)
                TargetY = float.Parse(parts[5].Split(':')[1]);

                // Add parsing for any additional stats here (e.g., Health = int.Parse(parts[6].Split(':')[1]);)
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing U_POS message: {rawMessage}. Error: {ex.Message}");
                throw; // Re-throw to indicate a critical parsing failure
            }
        }

        // --- CONSTRUCTOR 2 (Optional): For converting the object back to a string ---
        // This is useful for testing or if the client needs to echo state back.
        public string ToProtocolString()
        {
            return $"{COMMAND_CODE}|ID:{UnitID}|X:{PositionX}|Y:{PositionY}|TargetX:{TargetX}|TargetY:{TargetY}";
        }
    }
}