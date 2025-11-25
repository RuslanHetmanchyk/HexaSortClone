using System.Collections.Generic;
using System.Linq;
using Controller;
using Core.Helpers;
using Core.Services.Gameplay.Level.Implementation;
using Core.Services.Gameplay.Level.Interfaces;
using Cysharp.Threading.Tasks;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Level
{
    public class HexGridView : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private HexCellView cellPrefab;

        [Header("Layout")]
        [SerializeField] private float cellSize = 1f;

        private readonly Dictionary<Vector2Int, HexCellView> cellViews = new();

        private ILevelService levelService;
        private HexCellViewPool hexCellViewPool;

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
            ILevelService levelService,
            HexCellViewPool hexCellViewPool)
        {
            this.levelService = levelService;
            this.hexCellViewPool = hexCellViewPool;
        }

        private void Generate(List<HexCell> cells)
        {
            foreach (var pair in cellViews)
            {
                hexCellViewPool.Despawn(pair.Value);
            }

            cellViews.Clear();

            foreach (var hexCell in cells)
            {
                var world = HexHelper.AxialToWorld(hexCell.GridPosition, cellSize);

                var hexCellView = hexCellViewPool.Spawn(hexCell);
                hexCellView.transform.SetParent(transform, false);
                hexCellView.transform.SetPositionAndRotation(world, Quaternion.identity);

                hexCellView.Lock(hexCell.LockType, hexCell.LockValue);

                cellViews.Add(hexCell.GridPosition, hexCellView);
            }
        }

        private void ProcessMerge(HexCell cell)
        {
            ProcessMergeAsync(cell).Forget();
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

            var targetHexItemWorldPosition = cellStackView.NextItemWorldPosition();

            var hexItemView = neighborStackView.Pop();
            cellStackView.Push(hexItemView);

            hexItemView.Animator.PlayMoveAnimation(targetHexItemWorldPosition).Forget();
            await UniTask.Delay(100);

            await ProcessMergeHexItemAsync(toCell, fromCell);
        }

        private async UniTask ProcessBurnHexItemsAsync(Vector2Int gridPosition, int amount)
        {
            await UniTask.Delay(250);

            for (var i = 0; i < amount; i++)
            {
                cellViews[gridPosition].HexStackView.DestroyTopHexItemAsync().Forget();

                await UniTask.Delay(100);
            }
        }

        private void UnlockCell(Vector2Int obj)
        {
            cellViews[obj].Unlock();
        }
    }
}