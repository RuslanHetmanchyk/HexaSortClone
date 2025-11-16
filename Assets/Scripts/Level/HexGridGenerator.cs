using System.Collections.Generic;
using System.Linq;
using Core.Helpers;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Level.Interfaces;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using UnityEngine;
using Zenject;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private HexCellView cellPrefab;

    [Header("Layout")]
    [SerializeField] private float cellSize = 1f;

    [Space(10)]
    [SerializeField] private HexItemMover mover;

    private readonly Dictionary<Vector2Int, HexCellView> cellViews = new();
    
    private ICommandExecutionService commandService;
    private ILevelService levelService;

    private void Start()
    {
        var level = levelService.Load();
        Generate(level.Values.ToList());

        levelService.OnMergePossible += ProcessMerge;
        levelService.OnCellUnlocked += UnlockCell;
    }

    private void OnDestroy()
    {
        levelService.OnMergePossible -= ProcessMerge;
        levelService.OnCellUnlocked -= UnlockCell;
    }

    [Inject]
    private void Install(
        ICommandExecutionService commandService,
        ILevelService levelService)
    {
        this.commandService = commandService;
        this.levelService = levelService;
    }

    private void Generate(List<HexCell> cells)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        cellViews.Clear();

        foreach (var hexCell in cells)
        {
            var world = HexHelper.AxialToWorld(hexCell.GridPosition, cellSize);
            var hexCellView = Instantiate(cellPrefab, world, Quaternion.identity, transform);
            hexCellView.GridPos = hexCell.GridPosition;

            if (hexCell.Stack.Items.Count > 0)
            {
                hexCellView.Init(hexCell.Stack);
                hexCellView.Setup(commandService);
            }

            hexCellView.Lock(hexCell.LockType, hexCell.LockValue);

            cellViews.Add(hexCell.GridPosition, hexCellView);
        }
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

        var hexesToMerge = levelService.TryFindHexesToMerge(cell);
        if (hexesToMerge.Count > 0)
        {
            foreach (var neighbor in hexesToMerge)
            {
                await ProcessMergeHexItemAsync(cell, neighbor);
            }

            foreach (var neighbor in hexesToMerge)
            {
                await ProcessMergeAsync(neighbor);
            }
        }

        var countToRemove = levelService.TryBurn(cell);
        if (countToRemove > 0)
        {
            await ProcessBurnHexItemsAsync(cell.GridPosition, countToRemove);
            await ProcessMergeAsync(cell);
        }
        
        Debug.LogError("END");
    }

    private async UniTask ProcessMergeHexItemAsync(HexCell toCell, HexCell fromCell)
    {
        var success = levelService.TryMoveItem(fromCell, toCell);
        if (!success)
        {
            return;
        }

        var cellStackView = cellViews[toCell.GridPosition].HexStackView;
        var neighborStackView = cellViews[fromCell.GridPosition].HexStackView;

        var targetHexItemPosition = cellStackView.NextItemPosition();

        var hexItemView = neighborStackView.Pop();
        cellStackView.Push(hexItemView);

        await mover.PlayMoveAnimation(hexItemView.transform, targetHexItemPosition);

        await ProcessMergeHexItemAsync(toCell, fromCell);
    }

    private async UniTask ProcessBurnHexItemsAsync(Vector2Int gridPosition, int amount)
    {
        await UniTask.Delay(250);

        for (var i = 0; i < amount; i++)
        {
            var stackItem3D = cellViews[gridPosition].HexStackView.Pop();

            mover.DestroyAnimation(stackItem3D.transform);

            await UniTask.Delay(100);
        }
    }

    private void UnlockCell(Vector2Int obj)
    {
        cellViews[obj].Unlock();
    }
}