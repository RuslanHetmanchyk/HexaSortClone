using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public HexCell3D cellPrefab; // should have HexCell component
    public StackItem3D itemPrefab; // should have HexItem component or renderer to color

    [Header("Layout")]
    public float cellSize = 1f;         // hex spacing
    public float itemHeightOffset = 0.25f; // vertical offset between stacked items (Y)
    
    [Space(10)]
    public HexItemMover mover;
    
    public Dictionary<Vector2Int, HexCell3D> Cells = new ();

    private void Start()
    {
        var level = LevelService.Instance.Load();
        Generate(level.Values.ToList());

        LevelService.Instance.OnMatchingStacks += MergeStacks;
        LevelService.Instance.OnMergePossible += MergeStacks;
        LevelService.Instance.OnItemsBurned += BurnItems;
        LevelService.Instance.OnCellUnlocked += UnlockCell;
        
        
    }




    private void OnDestroy()
    {
        LevelService.Instance.OnMatchingStacks -= MergeStacks;
    }

    public void Generate(List<HexCell> cells)
    {
        // cleanup previous children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        Cells.Clear();

        foreach (var hexCell in cells)
        {
            Vector3 world = HexHelper.AxialToWorld(hexCell.GridPosition, cellSize);
            var cellObj = Instantiate(cellPrefab, world, Quaternion.identity, transform);
            cellObj.GridPos = hexCell.GridPosition;

            if (hexCell.Stack.Items.Count > 0)
            {
                cellObj.Init(hexCell.Stack);
            }
            cellObj.Lock(hexCell.LockType, hexCell.LockValue);
            
            Cells.Add(hexCell.GridPosition, cellObj);
        }
    }
    
    private void MergeStacks(HexCell sourceHexCell)
    {
        var hexesToMerge = LevelService.Instance.TryFindHexesToMerge(sourceHexCell);
        if (hexesToMerge?.Count > 0)
        {
            Merge(sourceHexCell, hexesToMerge);
        }
    }

    private async UniTask Merge(HexCell sourceHexCell, List<HexCell> targetHexCells)
    {
        foreach (var hexCell in targetHexCells)
        {
            if (LevelService.Instance.TryMoveItem(hexCell, sourceHexCell))
            {
                // Здесь await теперь работает, потому что Merge возвращает UniTask
                await MergeStacksAsync2(hexCell, sourceHexCell); 
            }
        }
    }
    
    // Изменили UniTaskVoid на UniTask, чтобы метод можно было ожидать (await)
    private async UniTask MergeStacksAsync2(HexCell sourceHexCell, HexCell targetHexCell)
    {
        bool success = LevelService.Instance.TryMoveItem(sourceHexCell, targetHexCell);
        if (!success)
        {
            var s = TryFindCellsToMerge(sourceHexCell); 
            if (!s)
            {
                Debug.LogError($"Try Burn");
                LevelService.Instance.TryBurn();
            }
            // завершение рекурсии
            return; // Возвращаем завершенный UniTask
        }

        // ... (логика анимации и перемещения)

        // задержка 0.5 сек (250 мс)
        await UniTask.Delay(250);

        // рекурсивный вызов с await для последовательности
        await MergeStacksAsync2(sourceHexCell, targetHexCell);
    }
    
    private void MergeStacks(HexCell sourceHexCell, HexCell targetHexCell)
    {
        // запустить рекурсивный процесс
        MergeStacksAsync(sourceHexCell, targetHexCell).Forget();
    }
    
    
    private async UniTask MergeStacksAsync(HexCell sourceHexCell, HexCell targetHexCell)
    {
        // Пытаемся переместить следующий предмет из source в target
        bool success = LevelService.Instance.TryMoveItem(sourceHexCell, targetHexCell);
    
        if (!success)
        {
            // Не удалось переместить (стек source пуст или условие не выполнено)
            var s = TryFindCellsToMerge(sourceHexCell); 
            if (!s)
            {
                Debug.LogError($"Try Burn");
                LevelService.Instance.TryBurn();
            }
            // Завершение рекурсии
            return; 
        }

        // Успешный мердж → проигрываем анимацию

        // Получаем представления стеков (предположительно, они кэшированы в Cells)
        var sourceStackView = Cells[sourceHexCell.GridPosition].Stack;
        var targetStackView = Cells[targetHexCell.GridPosition].Stack;

        // Извлекаем item 3D-объект, который только что был перемещен
        var stackItem3D = sourceStackView.Pop();

        // Определяем, куда он должен приземлиться в target
        var targetItemPosition = targetStackView.NextItemPosition();
    
        // Добавляем 3D-объект в список targetStackView (это важно для логики)
        targetStackView.items.Add(stackItem3D); 

        // Привязываем объект к targetStackView
        stackItem3D.transform.SetParent(targetStackView.transform);
    
        // Запускаем и ожидаем завершение анимации
        mover.PlayMoveAnimation(stackItem3D.transform, targetItemPosition);

        // задержка 0.25 сек перед следующим рекурсивным вызовом
        await UniTask.Delay(250);

        // Рекурсивный вызов с await для последовательного выполнения
        await MergeStacksAsync(sourceHexCell, targetHexCell);
    }
    
    
    

    // private async UniTaskVoid MergeStacksAsync(HexCell sourceHexCell, HexCell targetHexCell)
    // {
    //     bool success = LevelService.Instance.TryMoveItem(sourceHexCell, targetHexCell);
    //     if (!success)
    //     {
    //         var s = TryFindCellsToMerge(sourceHexCell); ////////////
    //
    //         if (!s)
    //         {
    //             Debug.LogError($"Try Burn");
    //             LevelService.Instance.TryBurn();
    //         }
    //         // завершение рекурсии
    //         return;
    //     }
    //
    //     // успешный мердж → проигрываем анимацию
    //     var sourceStackView = Cells[sourceHexCell.GridPosition].Stack;
    //     var targetStackView = Cells[targetHexCell.GridPosition].Stack;
    //
    //     var stackItem3D = sourceStackView.Pop();
    //
    //     var targetItemPosition = targetStackView.NextItemPosition();
    //     Debug.LogError(targetItemPosition);
    //     
    //     targetStackView.items.Add(stackItem3D);
    //
    //     stackItem3D.transform.SetParent(targetStackView.transform);
    //     mover.PlayMoveAnimation(stackItem3D.transform, targetItemPosition);
    //
    //     // mover.MoveItem(
    //     //     stackItem3D.transform,
    //     //     stackItem3D.transform,
    //     //     targetItemPosition,
    //     //     Vector3.one);
    //
    //
    //     // задержка 0.5 сек
    //     await UniTask.Delay(250);
    //
    //     // рекурсивный вызов
    //     MergeStacksAsync(sourceHexCell, targetHexCell);
    // }

    private bool TryFindCellsToMerge(HexCell sourceHexCell)
    {
        var targetHexCell = LevelService.Instance.TryFindHexToMerge(sourceHexCell);
        if (targetHexCell != null)
        {
            MergeStacksAsync(sourceHexCell, targetHexCell).Forget();
            return true;
        }
        
        return false;
    }

    private void BurnItems(Vector2Int gridPosition, int amount)
    {
        Debug.LogError($"Burn {amount} {gridPosition}");
        BurnItemsAsync(gridPosition, amount).Forget();
    }
    
    private async UniTaskVoid BurnItemsAsync(Vector2Int gridPosition, int amount)
    {
        await UniTask.Delay(250);
        
        for (int i = 0; i < amount; i++)
        {
            // 2. Извлекаем (Pop) верхний предмет
            var stackItem3D = Cells[gridPosition].Stack.Pop();

            // 3. Запускаем анимацию уничтожения
            mover.DestroyAnimation(stackItem3D.transform); 

            // 4. Ждем 0.1 секунды (асинхронно, без блокировки)
            await UniTask.Delay(100);
        }
    }
    
    
    private void UnlockCell(Vector2Int obj)
    {
        Cells[obj].Unlock();
    }
}
