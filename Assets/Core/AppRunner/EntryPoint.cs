using Core.Services.CommandRunner.Interfaces;
using Core.Services.Scenes.Commands;
using Core.Services.UI.Commands;
using UI.Screens;
using Zenject;

namespace Core.AppRunner
{
    public class EntryPoint : IInitializable
    {
        private readonly ICommandExecutionService commandExecutionService;

        public EntryPoint(ICommandExecutionService commandExecutionService)
        {
            this.commandExecutionService = commandExecutionService;
        }

        public void Initialize()
        {
            var loadLevelData = new LoadLevelData { SceneName = "StartScene" };
            //commandExecutionService.Execute<LoadLevelCommand<LoadLevelData>, LoadLevelData>(loadLevelData);
            commandExecutionService.Execute<ShowScreenCommand<StartScreen>>();
        }
    }
}