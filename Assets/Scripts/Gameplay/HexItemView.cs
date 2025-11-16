using Helpers;
using UnityEngine;

public class HexItemView : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private float height = 0.35f;

    public float Height => height;

    public void ApplyColorById(int idx)
    {
        if (rend.sharedMaterial != null)
        {
            rend.material.color = ColorHelper.Palette[idx];
        }
    }
}