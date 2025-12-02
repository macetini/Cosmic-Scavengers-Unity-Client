using UnityEngine;

namespace CosmicScavengers.Systems.MapGeneration.Noise
{
    /// <summary>
    /// Static utility for generating coherent, repeatable noise used for 
    /// procedural map generation. This version provides 2D noise for generating
    /// a classic height map.
    /// </summary>
    public static class NoiseGenerator
    {
        // Internal data structure for permutation table, seeded later.
        private static int[] _perm;
        private static int _seed;

        /// <summary>
        /// Initializes the permutation table based on the map seed.
        /// MUST be called once before any noise calculation.
        /// </summary>
        public static void SetSeed(long mapSeed)
        {
            // Use the absolute value of the long seed, cast to int.
            _seed = (int)(mapSeed & 0x7FFFFFFF); 
            
            // Create a pseudo-random number generator specific to this seed.
            System.Random rand = new(_seed);
            _perm = new int[512];
            int[] temp = new int[256];

            // Initialize temp array with 0-255
            for (int i = 0; i < 256; i++)
            {
                temp[i] = i;
            }

            // Shuffle the temp array using the seeded RNG
            for (int i = 0; i < 256; i++)
            {
                int swapIndex = rand.Next(i, 256);
                (temp[i], temp[swapIndex]) = (temp[swapIndex], temp[i]);
            }

            // Duplicate the array to avoid bounds checks later
            for (int i = 0; i < 256; i++)
            {
                _perm[i] = _perm[i + 256] = temp[i];
            }

            Debug.Log($"[Noise] NoiseGenerator initialized with seed: {_seed}");
        }

        // Simplex Noise 2D implementation (simplified for clarity)
        
        /// <summary>
        /// Generates a 2D Simplex Noise value between 0.0 and 1.0 for the given coordinates.
        /// This is used to define the height/elevation of the terrain.
        /// </summary>
        /// <param name="x">World X coordinate.</param>
        /// <param name="y">World Y coordinate.</param>
        /// <param name="scale">Factor to zoom in/out of the noise map (e.g., 0.01 for large features).</param>
        /// <param name="octaves">Number of layers of noise to combine for detail (Fractal Brownian Motion).</param>
        /// <param name="persistence">How quickly amplitude decays for higher octaves (e.g., 0.5).</param>
        /// <returns>Noise value between 0.0 (low elevation) and 1.0 (high elevation).</returns>
        public static float GetSimplexNoise(float x, float y, float scale, int octaves, float persistence)
        {
            if (_perm == null)
            {
                Debug.LogError("[Noise] NoiseGenerator not seeded! Call SetSeed() first.");
                return 0f;
            }

            float total = 0;
            float maxVal = 0;
            float amplitude = 1;
            float frequency = scale;

            // Combine multiple octaves (layers) of noise for detail (Fractal Brownian Motion)
            for (int i = 0; i < octaves; i++)
            {
                total += RawSimplex2D(x * frequency, y * frequency) * amplitude;
                maxVal += amplitude;
                amplitude *= persistence; // Decrease amplitude for next octave
                frequency *= 2;         // Double frequency (zoom in) for next octave
            }

            // Normalize the total to the 0.0 to 1.0 range
            return Mathf.Clamp01(total / maxVal);
        }

        // Stand-in for Simplex 2D noise implementation
        private static float RawSimplex2D(float x, float y)
        {
            // --- Stand-in for Simplex Noise ---
            // We use the seed for coordinate offset to ensure the result is unique to the mapSeed.
            float result = Mathf.PerlinNoise(
                x + _seed, 
                y + _seed
            );
            return result * 2f - 1f; // Adjust to roughly -1.0 to 1.0 range (Perlin is 0-1)
            // --- End Stand-in ---
        }
    }
}