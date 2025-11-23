using Core.Services.Gameplay.Level.Implementation;
using Helpers;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class HexItemView : MonoBehaviour, IPoolable<HexItem, IMemoryPool>
    {
        [SerializeField] private HexItemAnimator animator;
        [SerializeField] private Renderer rend;
        [SerializeField] private float height = 0.35f;

        private IMemoryPool pool;

        public HexItemAnimator Animator => animator;
        public float Height => height;

        public void ApplyColorById(int idx)
        {
            if (rend.sharedMaterial != null)
            {
                rend.material.color = ColorHelper.Palette[idx];
            }
        }

        public void Init(HexItem model)
        {
            ApplyColorById(model.ColorId);
        }

        public void OnDespawned()
        {
            gameObject.SetActive(false);
            pool = null;
        }

        public void OnSpawned(HexItem model, IMemoryPool pool)
        {
            this.pool = pool;
            Init(model);
            gameObject.SetActive(true);
        }
    }
}