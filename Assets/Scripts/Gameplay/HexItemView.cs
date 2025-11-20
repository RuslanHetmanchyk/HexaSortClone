using Helpers;
using UnityEngine;

namespace Gameplay
{
    public class HexItemView : MonoBehaviour
    {
        [SerializeField] private HexItemAnimator animator;
        [SerializeField] private Renderer rend;
        [SerializeField] private float height = 0.35f;

        public HexItemAnimator Animator => animator;
        public float Height => height;

        public void ApplyColorById(int idx)
        {
            if (rend.sharedMaterial != null)
            {
                rend.material.color = ColorHelper.Palette[idx];
            }
        }
    }
}