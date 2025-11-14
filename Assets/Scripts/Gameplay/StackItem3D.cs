using UnityEngine;

public class StackItem3D : MonoBehaviour
{
    //public MeshRenderer mesh;
    public float height = 0.35f;

    //public Color[] colors;

    // public void SetColor(int id)
    // {
    //     mesh.material.color = colorValues[id];
    // }
    //
    // private static readonly string[] colorNames = { "Red", "Green", "Blue", "Yellow", "Purple", "Cyan", "Orange" };
    // private static readonly Color[] colorValues = {
    //     new Color(0.85f,0.18f,0.18f),
    //     new Color(0.18f,0.70f,0.18f),
    //     new Color(0.18f,0.45f,0.85f),
    //     new Color(0.95f,0.85f,0.18f),
    //     new Color(0.62f,0.18f,0.85f),
    //     new Color(0.18f,0.80f,0.85f),
    //     new Color(0.95f,0.58f,0.18f)
    // };
    //
    private Renderer rend;

    // palette must match editor palette order
    private static readonly Color[] palette = {
        new Color(0.85f,0.18f,0.18f), // Red
        new Color(0.18f,0.70f,0.18f), // Green
        new Color(0.18f,0.45f,0.85f), // Blue
        new Color(0.95f,0.85f,0.18f), // Yellow
        new Color(0.62f,0.18f,0.85f), // Purple
        new Color(0.18f,0.80f,0.85f), // Cyan
        new Color(0.95f,0.58f,0.18f)  // Orange
    };

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        //ApplyColorByIndex(ColorId);
    }

    public void ApplyColorById(int idx)
    {
        Color c = palette[Mathf.Clamp(idx, 0, palette.Length - 1)];
        if (rend == null) rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (rend.sharedMaterial != null) // modify instance
            {
                rend.material.color = c;
            }
        }
    }
}