using Core.Services.CommandRunner.Interfaces;
using Zenject;

namespace Core.Services.CommandRunner.Implementation
{
    public class CommandExecutionServiceInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<ICommandFactory>()
                .To<CommandFactory>()
                .FromNew()
                .AsSingle();
            
            Container
                .Bind<ICommandExecutionService>()
                .To<CommandExecutionService>()
                .FromNew()
                .AsSingle();
        }
    }
}