using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public HexCell3D cellPrefab; // should have HexCell component

    [Header("Layout")]
    public float cellSize = 1f;         // hex spacing
    
    [Space(10)]
    public HexItemMover mover;
    
    public Dictionary<Vector2Int, HexCell3D> Cells = new ();

    private void Start()
    {
        var level = LevelService.Instance.Load();
        Generate(level.Values.ToList());

        LevelService.Instance.OnMergePossible += ProcessMerge;
        LevelService.Instance.OnCellUnlocked += UnlockCell;
    }

    private void OnDestroy()
    {
        LevelService.Instance.OnMergePossible -= ProcessMerge;
        LevelService.Instance.OnCellUnlocked -= UnlockCell;
    }

    private void Generate(List<HexCell> cells)
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
    
    private void UnlockCell(Vector2Int obj)
    {
        Cells[obj].Unlock();
    }


    private void ProcessMerge(HexCell cell)
    {
        ProcessMergeAsync(cell);
    }

    private async UniTask ProcessMergeAsync(HexCell cell)
    {
        if (cell.Stack?.Items.Count == 0)
        {
            return;
        }
        
        // 1. Пытаемся переместить айтемы из соседей
        var hexesToMerge = LevelService.Instance.TryFindHexesToMerge(cell);
        if (hexesToMerge.Count > 0)
        {
            foreach (var neighbor in hexesToMerge)
            {
                await ProcessMergeAsync(cell, neighbor);
            }
            
            foreach (var neighbor in hexesToMerge)
            {
                await ProcessMergeAsync(neighbor);
            }
        }

        // 3. Если изменений не было → проверяем на сжигание
        var countToRemove = LevelService.Instance.TryBurn(cell);
        if (countToRemove > 0)
        {
            await BurnItemsAsync(cell.GridPosition, countToRemove);
            await ProcessMergeAsync(cell);
        }
    }

    private async UniTask ProcessMergeAsync(HexCell toCell, HexCell fromCell)
    {
        bool success = LevelService.Instance.TryMoveItem(fromCell, toCell);
        if (!success)
        {
            return;
        }
        
        // Получаем представления стеков
        var cellStackView = Cells[toCell.GridPosition].Stack;
        var neighborStackView = Cells[fromCell.GridPosition].Stack;

        // Извлекаем item 3D-объект, который только что был перемещен
        var stackItem3D = neighborStackView.Pop();

        // Определяем, куда он должен приземлиться в target
        var targetItemPosition = cellStackView.NextItemPosition();
    
        // Добавляем 3D-объект в список cellStackView
        cellStackView.items.Add(stackItem3D); 

        // Привязываем объект к cellStackView
        stackItem3D.transform.SetParent(cellStackView.transform);
            
        // Запускаем и ожидаем завершение анимации
        await mover.PlayMoveAnimation(stackItem3D.transform, targetItemPosition);
        
        await ProcessMergeAsync(toCell, fromCell);
    }
    
    private async UniTask BurnItemsAsync(Vector2Int gridPosition, int amount)
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
}
