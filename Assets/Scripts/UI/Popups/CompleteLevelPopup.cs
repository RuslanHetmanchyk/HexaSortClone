using Core.Services.CommandRunner.Interfaces;
using Core.Services.Scenes.Commands;
using Core.Services.UI.Implementation.UIUnits;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Popups
{
    public class CompleteLevelPopup : UIPopup
    {
        [SerializeField] private Button buttonClose;
        
        private ICommandExecutionService commandExecutionService;

        [Inject]
        private void Install(ICommandExecutionService commandExecutionService)
        {
            this.commandExecutionService = commandExecutionService;
        }

        private void Start()
        {
            buttonClose.onClick.AddListener(() =>
            {
                var data = new LoadLevelData { SceneName = "StartScene" };
                commandExecutionService.Execute<LoadLevelCommand<LoadLevelData>, LoadLevelData>(data);
                
                Hide();
            });
        }
    }
}