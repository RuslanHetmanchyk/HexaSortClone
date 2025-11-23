using Gameplay;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class HexStackViewInstaller : MonoInstaller
    {
        [SerializeField] private HexStackView prefabHexStackView;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<HexStackView, HexStackViewPool>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(prefabHexStackView)
                .UnderTransformGroup("Pool(HexStackView)");
        }
    }
}