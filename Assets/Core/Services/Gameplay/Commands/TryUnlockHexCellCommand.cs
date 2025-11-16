using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.Gameplay.Level.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Services.Gameplay.Commands
{
    public class TryUnlockHexCellCommand : ICommandWithData<UnlockHexCellData>
    {
        private ILevelService levelService;

        [Inject]
        public void Construct(ILevelService levelService)
        {
            this.levelService = levelService;
        }

        public void Execute(UnlockHexCellData commandData)
        {
            levelService.TryUnlockCell(commandData.HexCellPosition);
        }
    }

    public struct UnlockHexCellData
    {
        public Vector2Int HexCellPosition;
    }
}