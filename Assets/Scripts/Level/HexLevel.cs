using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HexCellData
{
    public Vector2Int pos;       // axial coordinates (q, r)
    public bool active = false;
    public int cellColor = 0;    // индекс цвета
    public List<int> items = new (); // стек: first = bottom, last = top
    
    public LockType lockType = LockType.None;
    public int lockValue = 0; // значение для условий (кол-во рекламы, монет или гексов)
}

public enum LockType
{
    None,          // нет блокировки
    WatchAds,      // просмотр рекламы N раз
    PayCoins,      // покупка за N монет
    BurnGoal       // при сжигании N гексов
}

[CreateAssetMenu(fileName = "HexLevel", menuName = "HexaSort/Level")]
public class HexLevel : ScriptableObject
{
    [Range(1, 10)]
    public int radius = 3;

    // list of cells that cover full axial rectangle for given radius
    public List<HexCellData> cells = new ();
    
    public int hexesToBurnGoal = 150;

    // helper: ensure cells size matches radius and contains all coordinates
    public void EnsureGridInitialized()
    {
        int r = Mathf.Max(1, radius);
        var newCells = new List<HexCellData>();

        for (int q = -r; q <= r; q++)
        {
            int r1 = Mathf.Max(-r, -q - r);
            int r2 = Mathf.Min(r, -q + r);
            for (int s = r1; s <= r2; s++)
            {
                var cell = cells.Find(c => c.pos.x == q && c.pos.y == s);
                if (cell == null)
                {
                    cell = new HexCellData { pos = new Vector2Int(q, s), active = false, cellColor = 0, items = new List<int>() };
                }
                newCells.Add(cell);
            }
        }

        cells = newCells;
    }

    // axial distance from (0,0)
    public static int AxialDistance(Vector2Int a, Vector2Int b)
    {
        int dx = a.x - b.x;
        int dy = a.y - b.y;
        int dz = -a.x - a.y - (-b.x - b.y);
        return (Mathf.Abs(dx) + Mathf.Abs(dy) + Mathf.Abs(dz)) / 2;
    }
}