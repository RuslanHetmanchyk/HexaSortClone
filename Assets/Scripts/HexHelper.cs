using UnityEngine;

namespace DefaultNamespace
{
    public static class HexHelper
    {
        public static Vector3 AxialToWorld(Vector2Int hex, float size)
        {
            // pointy-top axial to world in XZ plane
            float x = size * (Mathf.Sqrt(3) * hex.x + Mathf.Sqrt(3) / 2f * hex.y);
            float z = size * (3f / 2f * hex.y);
            return new Vector3(x, 0f, z);
        }
    }
}