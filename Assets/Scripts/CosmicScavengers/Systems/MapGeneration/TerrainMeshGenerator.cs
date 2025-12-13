using System.Collections.Generic;
using Cosmic.Scavengers.Generated;
using CosmicScavengers.Systems.MapGeneration.Meta;
using CosmicScavengers.Systems.MapGeneration.Noise;
using UnityEngine;

namespace CosmicScavengers.Systems.MapGeneration
{
    /// <summary>
    /// Component responsible for generating a Unity Mesh from the procedural
    /// height data provided by the MapGenerator.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The side length of the square map chunk (e.g., 64x64 grid).")]
        private int mapSize = 64;

        [SerializeField]
        [Tooltip("The size of each grid square in world units.")]
        private float meshScale = 1.0f;

        [Header("Color Map Settings")]
        [Tooltip("Define color transitions from low to high elevation. MUST be sorted low to high.")]
        public List<TerrainType> terrainTypes = new();

        [Header("References")]
        [SerializeField]
        private MapGenerator mapGenerator;

        private MeshFilter meshFilter;
        private Mesh mesh;

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        /// <summary>
        /// Generates the mesh geometry based on the seeded MapGenerator data.
        /// </summary>
        public void GenerateMesh(WorldData mapData)
        {
            if (mapGenerator == null)
            {
                Debug.LogError("[TerrainMeshGenerator] MapGenerator reference is missing.");
                return;
            }
            if (terrainTypes == null || terrainTypes.Count == 0)
            {
                Debug.LogWarning("[TerrainMeshGenerator] Terrain Types list is empty. Mesh will be uncolored.");
            }

            mapGenerator.GenerateMap(mapData.MapSeed);

            Debug.Log($"[TerrainMeshGenerator] Generating {mapSize}x{mapSize} mesh...");

            Vector3[] vertices = new Vector3[mapSize * mapSize];
            int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
            Vector2[] uvs = new Vector2[mapSize * mapSize];
            Color[] colors = new Color[mapSize * mapSize];

            int vertIndex = 0;
            int triIndex = 0;

            float offsetX = mapSize * meshScale * 0.5f;
            float offsetZ = mapSize * meshScale * 0.5f;

            // 1. Generate Vertices, UVs, and COLORS
            for (int z = 0; z < mapSize; z++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    float worldX = x * meshScale;
                    float worldZ = z * meshScale;

                    // Query the elevation from the seeded MapGenerator (this is the Y component)
                    float elevation = mapGenerator.GetElevation(worldX, worldZ);

                    //Debug.Log("Elevation at (" + worldX + ", " + worldZ + ") is " + elevation);

                    // Set Vertex Position
                    vertices[vertIndex] = new Vector3(worldX - offsetX, elevation, worldZ - offsetZ);

                    // Set UVs for texture mapping (normalized 0 to 1)
                    uvs[vertIndex] = new Vector2((float)x / mapSize, (float)z / mapSize);

                    // --- NEW: Color assignment logic ---
                    Color vertexColor = Color.black;
                    for (int i = 0; i < terrainTypes.Count; i++)
                    {
                        // Compare the actual world elevation directly against the defined height threshold
                        if (elevation <= terrainTypes[i].height)
                        {
                            vertexColor = terrainTypes[i].color;
                            break;
                        }
                    }
                    colors[vertIndex] = vertexColor;
                    // --- END NEW LOGIC ---

                    // 2. Generate Triangles (Skip the last row and column)
                    if (x < mapSize - 1 && z < mapSize - 1)
                    {
                        int a = vertIndex;
                        int b = vertIndex + 1;
                        int c = vertIndex + mapSize + 1;
                        int d = vertIndex + mapSize;

                        // First triangle: (A, D, B)
                        triangles[triIndex + 0] = a;
                        triangles[triIndex + 1] = d;
                        triangles[triIndex + 2] = b;

                        // Second triangle: (B, D, C) -- (Note: Winding order corrected for Unity)
                        triangles[triIndex + 3] = b;
                        triangles[triIndex + 4] = d;
                        triangles[triIndex + 5] = c;

                        triIndex += 6;
                    }
                    vertIndex++;
                }
            }

            // 3. Apply Mesh Data
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors; // <-- APPLY THE NEW COLOR ARRAY!

            // 4. Recalculate properties (crucial for lighting and physics)
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Debug.Log("[TerrainMeshGenerator] Mesh generation and coloring complete.");
        }
    }
}