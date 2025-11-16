using UnityEngine;

namespace Core.Helpers
{
    public static class HexHelper
    {
        public static Vector2Int[] Directions =
        {
            new (+1, 0),   // East
            new (+1, -1),  // NE
            new (0, -1),   // NW
            new (-1, 0),   // West
            new (-1, +1),  // SW
            new (0, +1)    // SE
        };

        public static Vector3 AxialToWorld(Vector2Int hex, float size)
        {
            // pointy-top axial to world in XZ plane
            float x = size * (Mathf.Sqrt(3) * hex.x + Mathf.Sqrt(3) / 2f * hex.y);
            float z = size * (3f / 2f * hex.y);
            return new Vector3(x, 0f, z);
        }
    }
}