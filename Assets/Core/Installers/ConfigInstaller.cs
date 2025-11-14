using Core.Config;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    [CreateAssetMenu(
        fileName = "ConfigInstaller",
        menuName = "Installers/ConfigInstaller")]
    public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
    {
        [SerializeField] private ConfigAsset configAsset;

        public override void InstallBindings()
        {
            Container
                .Bind<ConfigAsset>()
                .FromInstance(configAsset);
        }
    }
}