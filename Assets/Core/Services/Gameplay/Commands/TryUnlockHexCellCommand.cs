using System;
using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.Gameplay.Level.Interfaces;
using Core.Services.UI.Interfaces;
using UI.Popups;
using UnityEngine;
using Zenject;

namespace Core.Services.Gameplay.Commands
{
    public class TryUnlockHexCellCommand : ICommandWithData<UnlockHexCellData>
    {
        private ILevelService levelService;
        private IUIService uiService;

        [Inject]
        public void Construct(
            ILevelService levelService,
            IUIService uiService)
        {
            this.levelService = levelService;
            this.uiService = uiService;
        }

        public void Execute(UnlockHexCellData commandData)
        {
            levelService.TryUnlockCell(commandData.HexCellPosition);

            var notificationPopup = uiService.GetPopup<NotificationPopup>();
            notificationPopup.SetLabel($"CELL UNLOCKED{Environment.NewLine}{commandData.HexCellPosition}");
            notificationPopup.Show();
        }
    }

    public struct UnlockHexCellData
    {
        public Vector2Int HexCellPosition;
    }
}