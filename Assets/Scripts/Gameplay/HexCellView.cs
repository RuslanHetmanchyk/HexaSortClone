using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Commands;
using Core.Services.Gameplay.Level.Implementation;
using UnityEngine;

namespace Gameplay
{
    public class HexCellView : MonoBehaviour
    {
        [SerializeField] private HexStackView hexStackPrefab;
        [SerializeField] private HexCellLockView lockView;

        public bool HasItems => HexStack?.Items?.Count > 0;

        public Vector2Int GridPos { get; set; }
        public HexStack HexStack { get; private set; }
        public HexStackView HexStackView { get; private set; }

        public LockType LockType { get; private set; }
        public int LockValue { get; private set; }
        
        private ICommandExecutionService commandService;

        private void OnMouseUpAsButton()
        {
            if (LockType == LockType.None)
            {
                return;
            }

            TryToUnlockHexCell();
        }

        public void Init(HexCell model)
        {
            HexStack = model.Stack;
            
            HexStackView = Instantiate(hexStackPrefab, transform);
            HexStackView.Init(model.Stack);
            HexStackView.SetDraggableActive(false);
        }

        public void Setup(ICommandExecutionService commandService)
        {
            this.commandService = commandService;
        }

        public bool CanAcceptStack()
        {
            return LockType == LockType.None && !HasItems;
        }

        public void PlaceStack(HexStackView hexStack)
        {
            HexStackView = hexStack;

            hexStack.transform.position = transform.position;
        }

        public void Lock(LockType lockType, int lockValue)
        {
            LockType = lockType;
            LockValue = lockValue;

            if (lockType == LockType.None)
            {
                return;
            }

            lockView.EnableLock(lockType, lockValue);
        }

        public void Unlock()
        {
            LockType = LockType.None;
            lockView.DisableLock();
        }

        private void TryToUnlockHexCell()
        {
            var data = new UnlockHexCellData
            {
                HexCellPosition = GridPos,
            };
            commandService.Execute<TryUnlockHexCellCommand, UnlockHexCellData>(data);
        }
    }
}