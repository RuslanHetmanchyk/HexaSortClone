using Gameplay;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class HexCellViewInstaller : MonoInstaller
    {
        [SerializeField] private HexCellView prefabHexCellView;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<HexCellView, HexCellViewPool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(prefabHexCellView)
                .UnderTransformGroup("Pool(HexCellView)");
        }
    }
}