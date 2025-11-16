using System;
using System.Collections.Generic;
using Core.Services.Gameplay.Level.Implementation;
using UnityEngine;

namespace Core.Services.Gameplay.Level.Interfaces
{
    public interface ILevelService
    {
        event Action OnStackSpawned;
        event Action<Vector2Int> OnCellUnlocked;
        event Action OnScoreChanged;
        event Action OnLevelGoalReached;
        event Action<HexCell> OnMergePossible;

        Dictionary<Vector2Int, HexCell> Cells { get; }
        int LevelScore { get; }

        List<HexStack> GeneratedStacks { get; }

        Dictionary<Vector2Int, HexCell> Load();


        void TryUnlockCell(Vector2Int gridPosition);
        void DropStackToCell(HexStack hexStack, Vector2Int gridPosition);

        List<HexCell> TryFindHexesToMerge(HexCell targetHexCell);
        bool TryMoveItem(HexCell sourceHexCell, HexCell targetHexCell);

        int TryBurn(HexCell hexCell);

        void GenerateRandomStacks();
        void RemoveGeneratedStack(HexStack stack);
    }
}