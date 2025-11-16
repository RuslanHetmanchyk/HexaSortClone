using Core.Services.Gameplay.Level.Implementation;
using Core.Services.Gameplay.Level.Interfaces;
using Zenject;

namespace Core.Services.Gameplay.Level
{
    public class LevelServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ILevelService>()
                .To<LevelService>()
                .FromNew()
                .AsSingle();
        }
    }
}