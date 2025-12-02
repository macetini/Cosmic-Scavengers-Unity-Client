using System.Text;
using CosmicScavengers.Core.Models;
using UnityEngine;

namespace CosmicScavengers.Systems.MapGeneration.Noise
{
    public class MapGenerator : MonoBehaviour
    {
        // Configuration for the noise function
        [Header("Noise Configuration")]
        [Tooltip("The 'zoom' level. Smaller values mean larger, smoother features.")]
        [Range(0.001f, 0.1f)]
        public float terrainScale = 0.015f;

        [Tooltip("Number of noise layers (octaves) for detail.")]
        [Range(1, 8)]
        public int octaves = 5;

        [Tooltip("Controls how much impact high-frequency (detailed) noise has.")]
        [Range(0.1f, 1f)]
        public float persistence = 0.5f;

        [Header("Height Map Parameters")]
        [Tooltip("Maximum vertical scale of the terrain (e.g., how high mountains can be).")]
        public float maxElevation = 100f;

        // World Data reference (should be set by a manager after connection)
        //private WorldData _currentWorldData;
        private bool _isSeeded = false;

        public void GenerateMap(long mapSeed)
        {
            //_currentWorldData = data;

            // 1. Initialize the static noise generator with the map seed
            NoiseGenerator.SetSeed(mapSeed);
            _isSeeded = true;

            //Debug.Log($"[MapGenerator] 2D Height Map system ready for World '{data.WorldName}' (Seed: {data.MapSeed}).");

            // Example check: get elevation at origin
            float elevation = GetElevation(0, 0);
            Debug.Log($"[MapGenerator] Elevation at (0, 0) is: {elevation:F3}");
            DrawDebugMap(100); // Draw a debug map slice
        }

        /// <summary>
        /// Calculates the actual elevation (height) in world units at a 
        /// specific 2D world coordinate.
        /// </summary>
        /// <param name="worldX">World X coordinate.</param>
        /// <param name="worldY">World Y coordinate.</param>
        /// <returns>A float value representing the height in world units (e.g., 0.0 to 100.0).</returns>
        public float GetElevation(float worldX, float worldY)
        {
            if (!_isSeeded)
            {
                Debug.LogError("[MapGenerator] Cannot generate map: World data not set or NoiseGenerator not seeded.");
                return 0f;
            }

            // Get the normalized noise value (0.0 to 1.0)
            float normalizedNoise = NoiseGenerator.GetSimplexNoise(
                worldX,
                worldY,
                terrainScale,
                octaves,
                persistence
            );

            // Scale the normalized noise to the desired world elevation range
            return normalizedNoise * maxElevation;
        }

        /// <summary>
        /// Example method for visualization: draws a character-based 2D map in the console.
        /// </summary>
        public void DrawDebugMap(int size)
        {
            if (!_isSeeded) return;

            StringBuilder sb = new();
            sb.AppendLine($"--- Debug Height Map ({size}x{size}) ---");

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float elevation = GetElevation(x, y);

                    // Simple character mapping based on height relative to maxElevation
                    char tile;
                    if (elevation < maxElevation * 0.1f) tile = '~'; // Water/Lowland
                    else if (elevation < maxElevation * 0.3f) tile = '='; // Plains
                    else if (elevation < maxElevation * 0.6f) tile = '^'; // Hills
                    else tile = '#'; // Mountains

                    sb.Append(tile);
                }
                sb.AppendLine();
            }

            Debug.Log($"[MapGenerator] Debug Height Map:\n{sb.ToString()}");
        }
    }
}