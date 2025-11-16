using Core.Config;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Level.Interfaces;
using Core.Services.Scenes.Commands;
using Core.Services.UI.Commands;
using Core.Services.UI.Implementation.UIUnits;
using Core.Services.User.Interfaces;
using TMPro;
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Screens
{
    public class GameplayScreen : UIScreen
    {
        [SerializeField] private TextMeshProUGUI labelScore;
        [SerializeField] private Button buttonClose;

        [SerializeField] private Button buttonBuster1;

        private ICommandExecutionService commandExecutionService;
        private ILevelService levelService;
        private IUserService userService;
        private ConfigAsset configAsset;

        [Inject]
        private void Install(
            ICommandExecutionService commandExecutionService,
            ILevelService levelService,
            IUserService userService,
            ConfigAsset configAsset)
        {
            this.commandExecutionService = commandExecutionService;
            this.levelService = levelService;
            this.userService = userService;
            this.configAsset = configAsset;
        }

        private void Start()
        {
            buttonClose.onClick.AddListener(ExitLevel);
            buttonBuster1.onClick.AddListener(UseBuster);
        }

        public override void Show()
        {
            base.Show();

            UpdateScore();

            levelService.OnScoreChanged += UpdateScore;
            levelService.OnLevelGoalReached += CompleteLevel;
        }

        public override void Hide()
        {
            base.Hide();

            levelService.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore()
        {
            labelScore.text = $"{levelService.LevelScore} / {configAsset.GetLevel(userService.Level).hexesToBurnGoal}";
        }

        private void CompleteLevel()
        {
            commandExecutionService.Execute<ShowPopupCommand<CompleteLevelPopup>>();
        }

        private void ExitLevel()
        {
            var data = new LoadLevelData { SceneName = "StartScene" };
            commandExecutionService.Execute<LoadLevelCommand<LoadLevelData>, LoadLevelData>(data);
        }

        private void UseBuster()
        {
            levelService.GenerateRandomStacks();
        }
    }
}