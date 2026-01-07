using System.Collections.Generic;
using CosmicScavengers.Core.Systems.Entities.Orchestrator;
using CosmicScavengers.Core.Systems.Terrain.Meta;
using CosmicScavengers.Gameplay.Networking.Event.Channels.Data;
using CosmicScavengers.Networking.Protobuf.WorldData;
using UnityEngine;

namespace CosmicScavengers.Systems.Terrain
{
    /// <summary>
    /// Component responsible for generating a Unity Mesh from the procedural
    /// height data provided by the MapGenerator.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class TerrainComponent : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The side length of the square map chunk (e.g., 64x64 grid).")]
        private int size = 64;

        [SerializeField]
        [Tooltip("The size of each grid square in world units.")]
        private float meshScale = 1.0f;

        [Header("Color Map Settings")]
        [Tooltip(
            "Define color transitions from low to high elevation. MUST be sorted low to high."
        )]
        public List<TerrainType> terrainTypes = new();

        [Header("References")]
        [SerializeField]
        private TerrainData terrainData;

        [SerializeField]
        private WorldDataChannel worldDataChannel;

        [SerializeField]
        [Tooltip("Reference to the EntityOrchestrator for entity management.")]
        private EntityOrchestrator entityOrchestrator;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        private Mesh mesh;

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        void Start()
        {
            if (entityOrchestrator == null)
            {
                throw new System.Exception(
                    "[TerrainComponent] EntityOrchestrator reference is missing."
                );
            }
            if (terrainData == null)
            {
                throw new System.Exception("[TerrainComponent] TerrainData reference is missing.");
            }
        }

        void OnEnable()
        {
            if (worldDataChannel != null)
            {
                worldDataChannel.AddListener(GenerateMesh);
            }
        }

        void OnDisable()
        {
            if (worldDataChannel != null)
            {
                worldDataChannel.RemoveListener(GenerateMesh);
            }
        }

        /// <summary>
        /// Generates the mesh geometry based on the seeded MapGenerator data.
        /// </summary>
        public void GenerateMesh(WorldData mapData)
        {
            if (terrainData == null)
            {
                Debug.LogError("[TerrainComponent] MapGenerator reference is missing.");
                return;
            }
            if (terrainTypes == null || terrainTypes.Count == 0)
            {
                Debug.LogWarning(
                    "[TerrainComponent] Terrain Types list is empty. Mesh will be uncolored."
                );
            }

            terrainData.GenerateData(mapData.MapSeed);

            Debug.Log($"[TerrainComponent] Generating {size}x{size} mesh...");

            Vector3[] vertices = new Vector3[size * size];
            int[] triangles = new int[(size - 1) * (size - 1) * 6];
            Vector2[] uvs = new Vector2[size * size];
            Color[] colors = new Color[size * size];

            int vertIndex = 0;
            int triIndex = 0;

            float offsetX = size * meshScale * 0.5f;
            float offsetZ = size * meshScale * 0.5f;

            // 1. Generate Vertices, UVs, and COLORS
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    float worldX = x * meshScale;
                    float worldZ = z * meshScale;

                    // Query the elevation from the seeded MapGenerator (this is the Y component)
                    float elevation = terrainData.GetElevation(worldX, worldZ);

                    //Debug.Log("Elevation at (" + worldX + ", " + worldZ + ") is " + elevation);

                    // Set Vertex Position
                    vertices[vertIndex] = new Vector3(
                        worldX - offsetX,
                        elevation,
                        worldZ - offsetZ
                    );

                    // Set UVs for texture mapping (normalized 0 to 1)
                    uvs[vertIndex] = new Vector2((float)x / size, (float)z / size);

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
                    if (x < size - 1 && z < size - 1)
                    {
                        int a = vertIndex;
                        int b = vertIndex + 1;
                        int c = vertIndex + size + 1;
                        int d = vertIndex + size;

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
            mesh.colors = colors;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;

            Debug.Log("[TerrainComponent] Mesh generation and coloring complete.");
        }
    }
}
