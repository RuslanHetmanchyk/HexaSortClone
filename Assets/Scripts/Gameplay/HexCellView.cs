using Controller;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Commands;
using Core.Services.Gameplay.Level.Implementation;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class HexCellView : MonoBehaviour
    {
        [SerializeField] private HexCellLockView lockView;

        public bool HasItems => HexStack?.Items?.Count > 0;

        public Vector2Int GridPos => hexCellModel.GridPosition;
        public HexStack HexStack => hexCellModel.Stack;
        public HexStackView HexStackView { get; private set; }

        public LockType LockType { get; private set; }
        public int LockValue { get; private set; }
        
        private ICommandExecutionService commandService;
        private HexStackViewPool hexStackViewPool;

        private HexCell hexCellModel;

        private void OnMouseUpAsButton()
        {
            if (LockType == LockType.None)
            {
                return;
            }

            TryToUnlockHexCell();
        }

        [Inject]
        private void Install(
            ICommandExecutionService commandService,
            HexStackViewPool hexStackViewPool)
        {
            this.commandService = commandService;
            this.hexStackViewPool = hexStackViewPool;
        }

        public void Init(HexCell model)
        {
            hexCellModel = model;
            
            if (hexCellModel.Stack.Items.Count > 0)
            {
                HexStackView = hexStackViewPool.Spawn(hexCellModel.Stack);
                HexStackView.transform.SetParent(transform, false);
                HexStackView.SetDraggableActive(false);
            }
        }

        public bool CanAcceptStack()
        {
            return LockType == LockType.None && !HasItems;
        }

        public void PlaceStack(HexStackView hexStack)
        {
            HexStackView = hexStack;
            HexStackView.transform.SetParent(transform, false);
            HexStackView.transform.localPosition = Vector3.zero;
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