using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelService : MonoBehaviour
{
    public static LevelService Instance { get; private set; }

    public Action OnStackSpawned;
    public Action<Vector2Int, int> OnItemsBurned;
    public Action<Vector2Int> OnCellUnlocked;
    public Action OnScoreChanged;
    public Action OnLevelGoalReached;
    public Action<HexCell> OnMergePossible;
    
    

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public string levelName = "Levels/Level01";
    
    public int LevelScore = 0;
    public int LevelGoal;
    public Dictionary<Vector2Int, HexCell> Cells = new ();

    public Dictionary<Vector2Int, HexCell> Load()
    {
        var level = Resources.Load<HexLevel>(levelName);
        if (level == null)
        {
            Debug.LogError("Level not found in Resources: " + levelName);
        }

        LevelGoal = level.hexesToBurnGoal;

        foreach (var cellConfig in level.cells)
        {
            if (cellConfig.active)
            {
                var items = cellConfig.items.Select(itemConfig => new StackItem(itemConfig)).ToList();
                var hexStack = new HexStack(items);
                
                var hexCell = new HexCell(cellConfig.pos, hexStack);
                hexCell.SetLock(cellConfig.lockType, cellConfig.lockValue);
                
                Cells.Add(cellConfig.pos, hexCell);
            }
        }

        return Cells;
    }


    public void DropStackToCell(HexStack hexStack, Vector2Int cellPosition)
    {
        var targetHexCell = Cells[cellPosition];
        targetHexCell.Stack = hexStack;
        
        CheckMergePossibility(targetHexCell);
    }

    private void CheckMergePossibility(HexCell cell)
    {
        var result = TryFindHexesToMerge(cell);
        if (result?.Count > 0)
        {
            OnMergePossible?.Invoke(cell);
        }
    }

    public HexCell TryFindHexToMerge(HexCell targetHexCell)
    {
        var neighbors = GetNotEmptyNeighbors(targetHexCell.GridPosition);
        return neighbors.FirstOrDefault(c => targetHexCell.Stack.Items.Count > 0 && c.Stack.Items.Last().ColorId == targetHexCell.Stack.Items.Last().ColorId);
    }
    
    public List<HexCell> TryFindHexesToMerge(HexCell targetHexCell)
    {
        var neighbors = GetNotEmptyNeighbors(targetHexCell.GridPosition);
        return neighbors.FindAll(c => c.Stack != null && 
                                      c.Stack.Items.Count > 0 && // Хотя это должно быть гарантировано GetNotEmptyNeighbors
                                      c.Stack.Items.Last().ColorId == targetHexCell.Stack.Items.Last().ColorId);
    }
    
    public List<HexCell> GetNotEmptyNeighbors(Vector2Int axialPos)
    {
        // 6 направлений в axial-координатах
        Vector2Int[] directions =
        {
            new Vector2Int(+1, 0),   // East
            new Vector2Int(+1, -1),  // NE
            new Vector2Int(0, -1),   // NW
            new Vector2Int(-1, 0),   // West
            new Vector2Int(-1, +1),  // SW
            new Vector2Int(0, +1)    // SE
        };

        var result = new List<HexCell>();

        foreach (var dir in directions)
        {
            var neighborPos = axialPos + dir;
            if (Cells.TryGetValue(neighborPos, out var neighbor))
            {
                if (neighbor.Stack.Items.Count > 0)
                {
                    result.Add(neighbor);
                }
            }
        }

        return result;
    }
    
    public List<HexStack> GeneratedStacks = new(3);
    public void GenerateRandomStacks()
    {
        GeneratedStacks.Clear();

        for (int i = 0; i < 3; i++)
        {
            GeneratedStacks.Add(new HexStack(GenerateRandomItems()));
        }
        
        OnStackSpawned?.Invoke();
    }

    public void RemoveGeneratedStack(HexStack stack)
    {
        var isSuccess = GeneratedStacks.Remove(stack);
        if (isSuccess && GeneratedStacks.Count == 0)
        {
            GenerateRandomStacks();
        }
    }
    
// предполагается, что есть конструктор: new StackItem(int colorId)
    private List<StackItem> GenerateRandomItems()
    {
        int itemCount = Random.Range(3, 7);   // 3..6
        int colorCount = Random.Range(1, 4);  // 1..3 distinct colors

        // 1) выбираем уникальные цвета
        List<int> chosen = new List<int>();
        while (chosen.Count < colorCount)
        {
            int c = Random.Range(0, 6);
            if (!chosen.Contains(c)) chosen.Add(c);
        }

        // 2) распределяем itemCount по цветам так, чтобы у каждого >=1
        // начнём с 1 для каждого
        int remaining = itemCount - colorCount;
        int[] counts = new int[colorCount];
        for (int i = 0; i < colorCount; i++) counts[i] = 1;

        // распределяем оставшиеся случайно между цветами
        for (int i = 0; i < remaining; i++)
        {
            int idx = Random.Range(0, colorCount);
            counts[idx]++;
        }

        // 3) собираем стопку блоками
        // можно перемешать порядок блоков для вариативности
        List<int> order = Enumerable.Range(0, colorCount).ToList();
        // случайно решить перемешивать или нет (чтобы иногда получить predictable order)
        if (Random.value > 0.5f)
            order = order.OrderBy(_ => Random.value).ToList();

        List<StackItem> items = new List<StackItem>();
        foreach (int idx in order)
        {
            int color = chosen[idx];
            int cnt = counts[idx];
            for (int k = 0; k < cnt; k++)
                items.Add(new StackItem(color));
        }

        return items;
    }
    
    public List<Vector2Int> ChangedCells = new List<Vector2Int>();

    public bool TryMoveItem(HexCell sourceHexCell, HexCell targetHexCell)
    {
        if (CanMoveItem(sourceHexCell, targetHexCell))
        {
            var stackItem = sourceHexCell.Stack.Pop();
            targetHexCell.Stack.Add(stackItem);
            
            ChangedCells.Add(targetHexCell.GridPosition);
            
            return true;
        }
        
        return false;
    }
    
    public bool CanMoveItem(HexCell sourceHexCell, HexCell targetHexCell)
    {
        if (sourceHexCell.Stack.Items.Count > 0 &&
            sourceHexCell.Stack.Items.Last().ColorId == targetHexCell.Stack?.Items.Last().ColorId)
        {
            return true;
        }
        
        return false;
    }

    public void TryBurn()
    {
        if (ChangedCells.Count == 0)
        {
            return;
        }

        var items = Cells[ChangedCells.Last()].Stack.Items;
        
        if (items == null || items.Count == 0)
            return;

        // Берём цвет верхнего (последнего) элемента
        int topColor = items.Last().ColorId;
        int countToRemove = 0;

        // Считаем, сколько подряд с конца имеют тот же цвет
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].ColorId == topColor)
                countToRemove++;
            else
                break;
        }

        // Удаляем найденное количество элементов
        if (countToRemove >= 8)
        {
            items.RemoveRange(items.Count - countToRemove, countToRemove);
            Debug.Log($"Removed {countToRemove} items of color {topColor}");
            
            IncreaseLevelScore(countToRemove);
            
            OnItemsBurned?.Invoke(Cells[ChangedCells.Last()].GridPosition, countToRemove);
        }
    }
    
    

    private void IncreaseLevelScore(int value)
    {
        LevelScore += value;
        OnScoreChanged?.Invoke();

        if (LevelScore >= LevelGoal)
        {
            OnLevelGoalReached?.Invoke();
        }
    }


    public void TryUnlockCell(Vector2Int gridPosition)
    {
        if (Cells.TryGetValue(gridPosition, out var cell))
        {
            cell.SetLock(LockType.None);
            OnCellUnlocked?.Invoke(gridPosition);
        }
    }
}