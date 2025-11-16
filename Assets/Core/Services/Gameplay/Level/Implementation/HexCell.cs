using UnityEngine;

namespace Core.Services.Gameplay.Level.Implementation
{
    public class HexCell
    {
        public HexStack Stack;
        public Vector2Int GridPosition { get; }
        public LockType LockType { get; private set; }
        public int LockValue { get; private set; }

        public HexCell(Vector2Int gridPosition, HexStack stack)
        {
            Stack = stack;
            GridPosition = gridPosition;
        }
        
        public void SetLock(LockType lockType, int lockValue = 0)
        {
            LockType = lockType;
            LockValue = lockValue;
        }
    }
}