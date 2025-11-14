using Zenject;

namespace Core.AppRunner
{
    public class EntryPointInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<EntryPoint>()
                .AsSingle()
                .NonLazy();
        }
    }
}