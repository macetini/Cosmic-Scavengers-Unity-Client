using UnityEngine;

namespace CosmicScavengers.Core.Systems.Terrain.Meta
{
    [System.Serializable]
    public struct TerrainType
    {
        [Tooltip("Name of the terrain type (e.g., Water, Sand, Grass, Mountain).")]
        public string name;

        [Tooltip("Normalized height value (0 to 1) where this terrain type begins.")]
        public float height;

        [Tooltip("Color used to represent this terrain type in debug maps.")]
        public Color color;
    }
}
