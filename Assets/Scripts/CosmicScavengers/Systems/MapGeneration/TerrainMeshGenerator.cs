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
        [Tooltip("The side length of the square map chunk (e.g., 64x64 grid).")]
        public int mapSize = 64;

        [Tooltip("The size of each grid square in world units.")]
        public float meshScale = 1.0f; 

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
        public void GenerateMesh(long seed)
        {
            if (mapGenerator == null)
            {
                Debug.LogError("[TerrainMeshGenerator] MapGenerator reference is missing.");
                return;
            }

            mapGenerator.GenerateMap(seed);

            Debug.Log($"[TerrainMeshGenerator] Generating {mapSize}x{mapSize} mesh...");

            Vector3[] vertices = new Vector3[mapSize * mapSize];
            int[] triangles = new int[(mapSize - 1) * (mapSize - 1) * 6];
            Vector2[] uvs = new Vector2[mapSize * mapSize];

            int vertIndex = 0;
            int triIndex = 0;

            // 1. Generate Vertices and UVs
            for (int z = 0; z < mapSize; z++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    // Calculate the world coordinates
                    float worldX = (x * meshScale);
                    float worldZ = (z * meshScale);

                    // Query the elevation from the seeded MapGenerator (this is the Y component)
                    float elevation = mapGenerator.GetElevation(worldX, worldZ);
                    
                    // Center the terrain grid visually
                    float offsetX = (mapSize * meshScale) / 2f;
                    float offsetZ = (mapSize * meshScale) / 2f;

                    vertices[vertIndex] = new Vector3(worldX - offsetX, elevation, worldZ - offsetZ);

                    // UVs for texture mapping (normalized 0 to 1)
                    uvs[vertIndex] = new Vector2((float)x / mapSize, (float)z / mapSize);

                    // 2. Generate Triangles (Skip the last row and column)
                    if (x < mapSize - 1 && z < mapSize - 1)
                    {
                        // Current vertex index
                        int a = vertIndex; 
                        // Vertex to the right
                        int b = vertIndex + 1; 
                        // Vertex down-right
                        int c = vertIndex + mapSize + 1; 
                        // Vertex below
                        int d = vertIndex + mapSize; 

                        // First triangle: (A, B, C)
                        triangles[triIndex + 0] = a;
                        triangles[triIndex + 1] = d;
                        triangles[triIndex + 2] = b; 
                        
                        // Second triangle: (C, D, B) -- Note: Order matters for face orientation (winding)
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
            
            // 4. Recalculate properties (crucial for lighting and physics)
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            Debug.Log("[TerrainMeshGenerator] Mesh generation complete.");
        }
    }
}