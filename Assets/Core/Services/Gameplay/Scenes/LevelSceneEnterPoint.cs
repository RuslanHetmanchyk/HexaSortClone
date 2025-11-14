using Core.Services.CommandRunner.Interfaces;
using Core.Services.UI.Commands;
using UI.Screens;
using UnityEngine;
using Zenject;

namespace Core.Services.Gameplay.Scenes
{
    public class LevelSceneEnterPoint : MonoBehaviour
    {
        private ICommandExecutionService commandExecutionService;

        [Inject]
        private void Install(ICommandExecutionService commandExecutionService)
        {
            this.commandExecutionService = commandExecutionService;
        }

        private void Start()
        {
            commandExecutionService.Execute<ShowScreenCommand<GameplayScreen>>();
        }
    }
}