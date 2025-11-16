using UnityEngine;

namespace Core.Helpers
{
    public static class HexHelper
    {
        public static Vector2Int[] Directions =
        {
            new(+1, 0),  // East
            new(+1, -1), // NE
            new(0, -1),  // NW
            new(-1, 0),  // West
            new(-1, +1), // SW
            new(0, +1)   // SE
        };

        public static Vector3 AxialToWorld(Vector2Int hex, float size)
        {
            // pointy-top axial to world in XZ plane
            var x = size * (Mathf.Sqrt(3) * hex.x + Mathf.Sqrt(3) / 2f * hex.y);
            var z = size * (3f / 2f * hex.y);
            return new Vector3(x, 0f, z);
        }

        public static int AxialDistance(Vector2Int a, Vector2Int b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            var dz = -a.x - a.y - (-b.x - b.y);
            return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
        }
    }
}