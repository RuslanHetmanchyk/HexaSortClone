using System;
using System.Collections.Generic;
using System.Linq;
using Core.Config;
using Core.Helpers;
using Core.Services.Gameplay.Level.Interfaces;
using Core.Services.User.Interfaces;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Core.Services.Gameplay.Level.Implementation
{
    public class LevelService : ILevelService
    {

    public event Action OnStackSpawned;
    public event Action<Vector2Int> OnCellUnlocked;
    public event Action OnScoreChanged;
    public event Action OnLevelGoalReached;
    public event Action<HexCell> OnMergePossible;
    
    public int LevelScore { get; private set; }
    public int LevelGoal;
    public Dictionary<Vector2Int, HexCell> Cells { get; private set; }

    private IUserService userService;
    private ConfigAsset configAsset;
    
    [Inject]
    private void Install(IUserService userService, ConfigAsset configAsset)
    {
        this.userService = userService;
        this.configAsset = configAsset;
    }

    public Dictionary<Vector2Int, HexCell> Load()
    {
        Cells = new Dictionary<Vector2Int, HexCell>();
        
        var level = configAsset.GetLevel(userService.Level);

        LevelGoal = level.hexesToBurnGoal;

        foreach (var cellConfig in level.cells)
        {
            if (cellConfig.active)
            {
                var items = cellConfig.items.Select(itemConfig => new HexItem(itemConfig)).ToList();
                var hexStack = new HexStack(items);
                
                var hexCell = new HexCell(cellConfig.pos, hexStack);
                hexCell.SetLock(cellConfig.lockType, cellConfig.lockValue);
                
                Cells.Add(cellConfig.pos, hexCell);
            }
        }

        return Cells;
    }


    public void DropStackToCell(HexStack hexStack, Vector2Int gridPosition)
    {
        var targetHexCell = Cells[gridPosition];
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
    
    public List<HexCell> TryFindHexesToMerge(HexCell targetHexCell)
    {
        if (targetHexCell.Stack?.Items?.Count == 0)
        {
            return new List<HexCell>();
        }

        var neighbors = GetNotEmptyNeighbors(targetHexCell.GridPosition);
        return neighbors.FindAll(c => c.Stack != null && 
                                      c.Stack.Items.Count > 0 && // Хотя это должно быть гарантировано GetNotEmptyNeighbors
                                      c.Stack.Items.Last().ColorId == targetHexCell.Stack.Items.Last().ColorId);
    }
    
    public List<HexCell> GetNotEmptyNeighbors(Vector2Int axialPos)
    {
        var result = new List<HexCell>();

        foreach (var dir in HexHelper.Directions)
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

    public List<HexStack> GeneratedStacks { get; private set; }
    public void GenerateRandomStacks()
    {
        GeneratedStacks = new List<HexStack>();

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
    private List<HexItem> GenerateRandomItems()
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

        List<HexItem> items = new List<HexItem>();
        foreach (int idx in order)
        {
            int color = chosen[idx];
            int cnt = counts[idx];
            for (int k = 0; k < cnt; k++)
                items.Add(new HexItem(color));
        }

        return items;
    }

    public bool TryMoveItem(HexCell sourceHexCell, HexCell targetHexCell)
    {
        if (CanMoveItem(sourceHexCell, targetHexCell))
        {
            var stackItem = sourceHexCell.Stack.Pop();
            targetHexCell.Stack.Add(stackItem);
            
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
    
    public int TryBurn(HexCell hexCell)
    {
        var items = hexCell.Stack.Items;

        if (items == null || items.Count == 0)
        {
            return 0;
        }

        // Берём цвет верхнего (последнего) элемента
        int topColor = items.Last().ColorId;
        int countToRemove = 0;

        // Считаем, сколько подряд с конца имеют тот же цвет
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].ColorId == topColor)
            {
                countToRemove++;
            }
            else
            {
                break;
            }
        }

        // Удаляем найденное количество элементов
        if (countToRemove >= 8)
        {
            items.RemoveRange(items.Count - countToRemove, countToRemove);
            
            IncreaseLevelScore(countToRemove);
        }
        
        return countToRemove;
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
}