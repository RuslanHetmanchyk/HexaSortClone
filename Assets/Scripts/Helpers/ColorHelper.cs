using UnityEngine;

namespace Helpers
{
    public static class ColorHelper
    {
        // palette must match editor palette order
        public static readonly Color[] Palette = {
            new Color(0.85f,0.18f,0.18f), // Red
            new Color(0.18f,0.70f,0.18f), // Green
            new Color(0.18f,0.45f,0.85f), // Blue
            new Color(0.95f,0.85f,0.18f), // Yellow
            new Color(0.62f,0.18f,0.85f), // Purple
            new Color(0.18f,0.80f,0.85f), // Cyan
            new Color(0.95f,0.58f,0.18f)  // Orange
        };
    }
}