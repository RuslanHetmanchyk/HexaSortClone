using Core.Services.UI.Implementation;
using Core.Services.UI.Interfaces;
using Zenject;

namespace Core.Services.UI
{
    public class UIServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<IUIFactory>()
                .To<UIFactory>()
                .FromNew()
                .AsSingle();

            Container
                .Bind<IUIService>()
                .To<UIService>()
                .FromNew()
                .AsSingle();
        }
    }
}