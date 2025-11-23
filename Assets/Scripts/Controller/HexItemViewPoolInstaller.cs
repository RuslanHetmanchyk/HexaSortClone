using Gameplay;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class HexItemViewPoolInstaller : MonoInstaller
    {
        [SerializeField] private HexItemView prefabHexItemView;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<HexItemView, HexItemViewPool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(prefabHexItemView)
                .UnderTransformGroup("Pool(HexItemView)");
        }
    }
}