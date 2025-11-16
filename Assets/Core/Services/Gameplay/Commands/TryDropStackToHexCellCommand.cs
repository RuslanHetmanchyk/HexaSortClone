using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.Gameplay.Level.Implementation;
using Core.Services.Gameplay.Level.Interfaces;
using UnityEngine;
using Zenject;

namespace Core.Services.Gameplay.Commands
{
    public class TryDropStackToHexCellCommand : ICommandWithData<StackDropData>
    {
        private ILevelService levelService;

        [Inject]
        public void Construct(ILevelService levelService)
        {
            this.levelService = levelService;
        }

        public void Execute(StackDropData commandData)
        {
            if (!levelService.Cells.TryGetValue(commandData.TargetCellPosition, out var targetCell))
            {
                Debug.LogError($"{commandData.TargetCellPosition} is not a valid cell");
                return;
            }

            levelService.RemoveGeneratedStack(commandData.HexStack);
            levelService.DropStackToCell(commandData.HexStack, commandData.TargetCellPosition);
        }
    }

    public struct StackDropData
    {
        public HexStack HexStack;
        public Vector2Int TargetCellPosition;
    }
}