using Core.Services.CommandRunner.Interfaces;
using Core.Services.Scenes.Commands;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.User.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class StartScreen : UIScreen
    {
        [SerializeField] private Button buttonStart;
        [SerializeField] private TextMeshProUGUI buttonLabel;

        private ICommandExecutionService commandExecutionService;
        private IUserService userService;

        [Inject]
        private void Install(
            ICommandExecutionService commandExecutionService,
            IUserService userService)
        {
            this.commandExecutionService = commandExecutionService;
            this.userService = userService;
        }

        private void Start()
        {
            buttonStart.onClick.AddListener(LoadNextLevel);
        }

        public override void Show()
        {
            base.Show();

            buttonLabel.text = $"Level {userService.Level}";
        }

        private void LoadNextLevel()
        {
            var data = new LoadLevelData
            {
                SceneName = "LevelScene"
            };
            commandExecutionService.Execute<LoadLevelCommand<LoadLevelData>, LoadLevelData>(data);
        }
    }
}