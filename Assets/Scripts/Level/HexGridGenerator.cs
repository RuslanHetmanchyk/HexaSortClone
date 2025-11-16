using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Gameplay;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private HexCellView cellPrefab;

    [Header("Layout")]
    [SerializeField] private float cellSize = 1f;

    [Space(10)]
    [SerializeField] private HexItemMover mover;

    private readonly Dictionary<Vector2Int, HexCellView> cellViews = new();

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

        var hexesToMerge = LevelService.Instance.TryFindHexesToMerge(cell);
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

        var countToRemove = LevelService.Instance.TryBurn(cell);
        if (countToRemove > 0)
        {
            await ProcessBurnHexItemsAsync(cell.GridPosition, countToRemove);
            await ProcessMergeAsync(cell);
        }
    }

    private async UniTask ProcessMergeHexItemAsync(HexCell toCell, HexCell fromCell)
    {
        var success = LevelService.Instance.TryMoveItem(fromCell, toCell);
        if (!success)
        {
            return;
        }

        var cellStackView = cellViews[toCell.GridPosition].HexStack;
        var neighborStackView = cellViews[fromCell.GridPosition].HexStack;

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
            var stackItem3D = cellViews[gridPosition].HexStack.Pop();

            mover.DestroyAnimation(stackItem3D.transform);

            await UniTask.Delay(100);
        }
    }

    private void UnlockCell(Vector2Int obj)
    {
        cellViews[obj].Unlock();
    }
}