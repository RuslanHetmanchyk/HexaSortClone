using DefaultNamespace;
using Gameplay;
using UnityEngine;

public class HexCell3D : MonoBehaviour
{
    [SerializeField] private Stack3D stackPrefab;
    [SerializeField] private HexCellLockView lockView;
    
    public bool active = true;
    public bool HasItems => LevelService.Instance.Cells[GridPos].Stack.Items.Count > 0;

    public Transform stackAnchor; // точка куда ставить стопку
    public Vector2Int GridPos { get; set; }
    public Stack3D Stack { get; private set; }
    
    public LockType LockType { get; private set; }
    public int LockValue { get; private set; }

    public void Init(HexStack stack)
    {
        Stack = Instantiate(stackPrefab, stackAnchor);
        Stack.Init(stack);
        Stack.SetDraggableActive(false);
    }

    public bool CanAcceptStack()
    {
        return active && LockType == LockType.None && !HasItems;
    }

    public void PlaceStack(Stack3D stack)
    {
        Stack = stack;
        
        stack.transform.position = stackAnchor.position;
        //HasItems = true;
        
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
    
    private void OnMouseUpAsButton()
    {
        if (LockType == LockType.None)
        {
            return;
        }
        
        Debug.LogError($"Clicked on: {GridPos}");

        TryToUnlock();
    }

    private void TryToUnlock()
    {
        LevelService.Instance.TryUnlockCell(GridPos);
        
    }
}