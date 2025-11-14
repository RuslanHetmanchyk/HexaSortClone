using Core.Services.CommandRunner.Interfaces.Command;
using Core.Services.UI.Interfaces;
using DefaultNamespace;
using UnityEngine;
using Zenject;

namespace Core.Services.Gameplay.Commands
{
    public class TryDropStackToHexCellCommand : ICommandWithData<StackDropData>
    {
        private IUIService uiService;

        [Inject]
        public void Construct(IUIService uiService)
        {
            this.uiService = uiService;
        }

        public void Execute(StackDropData commandData)
        {
            if (!LevelService.Instance.Cells.TryGetValue(commandData.TargetCellPosition, out var targetCell))
            {
                Debug.LogError($"{commandData.TargetCellPosition} is not a valid cell");
                return;
            }
        
            LevelService.Instance.RemoveGeneratedStack(commandData.HexStack);
            
            LevelService.Instance.DropStackToCell(commandData.HexStack, commandData.TargetCellPosition);
        }
    }

    public struct StackDropData
    {
        public HexStack HexStack;
        public Vector2Int TargetCellPosition;
    }
}