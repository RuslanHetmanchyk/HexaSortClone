using Core.Services.SaveLoad.Implementation;
using Core.Services.SaveLoad.Interfaces;
using Zenject;

namespace Core.Services.SaveLoad
{
    public class SaveLoadServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ISaveLoadService>()
                .To<PlayerPrefsSaveLoadService>()
                .FromNew()
                .AsSingle();
        }
    }
}