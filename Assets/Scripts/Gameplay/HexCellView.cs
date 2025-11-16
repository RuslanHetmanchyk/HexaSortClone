using DefaultNamespace;
using UnityEngine;

namespace Gameplay
{
    public class HexCellView : MonoBehaviour
    {
        [SerializeField] private HexStackView hexStackPrefab;
        [SerializeField] private HexCellLockView lockView;

        public bool HasItems => LevelService.Instance.Cells[GridPos].Stack.Items.Count > 0;

        public Vector2Int GridPos { get; set; }
        public HexStackView HexStack { get; private set; }

        public LockType LockType { get; private set; }
        public int LockValue { get; private set; }

        private void OnMouseUpAsButton()
        {
            if (LockType == LockType.None)
            {
                return;
            }

            TryToUnlock();
        }

        public void Init(HexStack stack)
        {
            HexStack = Instantiate(hexStackPrefab, transform);
            HexStack.Init(stack);
            HexStack.SetDraggableActive(false);
        }

        public bool CanAcceptStack()
        {
            return LockType == LockType.None && !HasItems;
        }

        public void PlaceStack(HexStackView hexStack)
        {
            HexStack = hexStack;

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

        private void TryToUnlock()
        {
            LevelService.Instance.TryUnlockCell(GridPos);
        }
    }
}