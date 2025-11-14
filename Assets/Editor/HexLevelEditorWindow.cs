using UnityEditor;
using UnityEngine;

public class HexLevelEditorWindow : EditorWindow
{
    private HexLevel level;
    private int selectedIndex = -1; // индекс в level.cells
    private Vector2 leftScroll, rightScroll;
    private float cellDrawSize = 24f;

    // fixed palette
    private static readonly string[] colorNames = { "Red", "Green", "Blue", "Yellow", "Purple", "Cyan", "Orange" };
    private static readonly Color[] colorValues = {
        new Color(0.85f,0.18f,0.18f),
        new Color(0.18f,0.70f,0.18f),
        new Color(0.18f,0.45f,0.85f),
        new Color(0.95f,0.85f,0.18f),
        new Color(0.62f,0.18f,0.85f),
        new Color(0.18f,0.80f,0.85f),
        new Color(0.95f,0.58f,0.18f)
    };

    [MenuItem("Tools/Hexa Sort Level Editor")]
    public static void OpenWindow()
    {
        var w = GetWindow<HexLevelEditorWindow>("Hex Level Editor");
        w.minSize = new Vector2(900, 450);
    }

    private void OnEnable()
    {
        // nothing for now
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        DrawLeftPanel();   // settings / selected cell editor on the left
        DrawRightPanel();  // grid drawing on the right

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        // bottom area: quick actions
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("New Empty Level Asset...", GUILayout.Width(200)))
        {
            CreateNewLevelAsset();
        }

        level = (HexLevel)EditorGUILayout.ObjectField("Level Asset", level, typeof(HexLevel), false);

        if (level != null)
        {
            if (GUILayout.Button("Create/Init Grid (radius)", GUILayout.Width(200)))
            {
                level.EnsureGridInitialized();
                AutoActivateRadius2();
                EditorUtility.SetDirty(level);
            }

            if (GUILayout.Button("Save Asset", GUILayout.Width(120)))
            {
                EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Auto-activate center + radius=2", GUILayout.Width(240)))
            {
                AutoActivateRadius2();
                EditorUtility.SetDirty(level);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

private void DrawLeftPanel()
{
    GUILayout.BeginVertical(GUILayout.Width(300));
    leftScroll = GUILayout.BeginScrollView(leftScroll);

    EditorGUILayout.LabelField("Cell Editor", EditorStyles.boldLabel);

    if (level == null)
    {
        EditorGUILayout.HelpBox("Select or create a HexLevel asset to edit.", MessageType.Info);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        return;
    }
    
    // ---- Level goal section ----
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Level Goal", EditorStyles.boldLabel);

    EditorGUI.BeginChangeCheck();
    int newGoal = EditorGUILayout.IntField("Hexes To Burn", level.hexesToBurnGoal);
    if (EditorGUI.EndChangeCheck())
    {
        level.hexesToBurnGoal = Mathf.Max(1, newGoal);
        EditorUtility.SetDirty(level);
    }
    EditorGUILayout.Space();

    // show level basic info
    EditorGUI.BeginChangeCheck();
    int newRadius = EditorGUILayout.IntField("Level Radius", level.radius);
    if (newRadius != level.radius)
    {
        level.radius = Mathf.Max(1, newRadius);
        level.EnsureGridInitialized();
        EditorUtility.SetDirty(level);
    }
    EditorGUILayout.Space();

    if (selectedIndex < 0 || selectedIndex >= level.cells.Count)
    {
        EditorGUILayout.HelpBox("Select a cell on the grid (right panel).", MessageType.None);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        return;
    }

    var cdata = level.cells[selectedIndex];

    EditorGUILayout.LabelField($"Cell {selectedIndex} — pos {cdata.pos.x},{cdata.pos.y}", EditorStyles.boldLabel);

    // ✅ ACTIVE TOGGLE ONLY
    bool oldActive = cdata.active;
    cdata.active = EditorGUILayout.Toggle("Active", cdata.active);
    EditorGUILayout.Space();

    // Disable items if cell is inactive
    EditorGUI.BeginDisabledGroup(!cdata.active);
    
    
    
    
    
    
    
    // ---- Lock settings ----
    EditorGUILayout.Space();
    EditorGUILayout.LabelField("Lock Settings", EditorStyles.boldLabel);

// Выбор типа блокировки
    cdata.lockType = (LockType)EditorGUILayout.EnumPopup("Lock Type", cdata.lockType);

// Если тип блокировки требует значения — показываем поле
    switch (cdata.lockType)
    {
        case LockType.WatchAds:
            cdata.lockValue = EditorGUILayout.IntField("Ads To Watch", Mathf.Max(1, cdata.lockValue));
            break;
        case LockType.PayCoins:
            cdata.lockValue = EditorGUILayout.IntField("Coins To Unlock", Mathf.Max(1, cdata.lockValue));
            break;
        case LockType.BurnGoal:
            cdata.lockValue = EditorGUILayout.IntField("Required Burned Hexes", Mathf.Max(1, cdata.lockValue));
            break;
    }
    EditorGUILayout.Space(15);
    
    
    
    
    
    
    
    

    // ✅ BUTTON "ADD ITEM" ABOVE THE LIST
    if (GUILayout.Button("Add Item"))
    {
        cdata.items.Add(0);
        EditorUtility.SetDirty(level);
    }

    EditorGUILayout.Space(5);
    EditorGUILayout.LabelField("Items (top → bottom):", EditorStyles.boldLabel);

// ✅ Рисуем от верха к низу (с конца списка к началу)
    for (int displayIndex = cdata.items.Count - 1; displayIndex >= 0; displayIndex--)
    {
        int realIndex = displayIndex; // настоящий индекс в списке

        EditorGUILayout.BeginHorizontal("box");

        int colorIndex = cdata.items[realIndex];

        // ◀ previous
        if (GUILayout.Button("◀", GUILayout.Width(28)))
            colorIndex = (colorIndex - 1 + colorNames.Length) % colorNames.Length;

        // dropdown
        colorIndex = EditorGUILayout.Popup(colorIndex, colorNames);

        // ▶ next
        if (GUILayout.Button("▶", GUILayout.Width(28)))
            colorIndex = (colorIndex + 1) % colorNames.Length;

        // X delete
        if (GUILayout.Button("X", GUILayout.Width(22)))
        {
            cdata.items.RemoveAt(realIndex);
            EditorUtility.SetDirty(level);
            EditorGUILayout.EndHorizontal();
            break;
        }

        // preview
        DrawColorRect(colorValues[colorIndex], 35, 18);

        cdata.items[realIndex] = colorIndex;

        EditorGUILayout.EndHorizontal();
    }


    EditorGUI.EndDisabledGroup(); // inactive cell

    // SET DIRTY
    if (GUI.changed || oldActive != cdata.active)
        EditorUtility.SetDirty(level);

    GUILayout.EndScrollView();
    GUILayout.EndVertical();
}


    private void DrawRightPanel()
    {
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Grid", EditorStyles.boldLabel);
        rightScroll = GUILayout.BeginScrollView(rightScroll);

        if (level == null)
        {
            EditorGUILayout.HelpBox("No level loaded.", MessageType.None);
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            return;
        }

        // ensure grid matches radius
        if (level.cells == null || level.cells.Count == 0)
            level.EnsureGridInitialized();

        // draw an area using Handles
        Rect area = GUILayoutUtility.GetRect(600, 600);
        Handles.BeginGUI();

        // center point
        Vector2 center = new Vector2(area.x + area.width / 2f, area.y + area.height / 2f);

        // draw each cell
        for (int i = 0; i < level.cells.Count; i++)
        {
            var c = level.cells[i];
            Vector2 pos = AxialToPixel(c.pos, center, cellDrawSize);

            // background: inactive = dim gray, active = cellColor
            Color bg = c.active ? colorValues[c.cellColor] : (Color.gray * 0.35f);
            Handles.color = bg;
            Handles.DrawSolidDisc(pos, Vector3.forward, cellDrawSize * 0.9f);

            // draw border
            Handles.color = Color.black;
            Handles.DrawWireDisc(pos, Vector3.forward, cellDrawSize * 0.95f);

            // draw small marker for stack size
            if (c.items != null && c.items.Count > 0)
            {
                // draw small squares representing stack (stack height)
                for (int si = 0; si < Mathf.Min(5, c.items.Count); si++)
                {
                    Vector2 sqPos = pos + new Vector2(0, -cellDrawSize * 1.2f - si * 8);
                    Rect r = new Rect(sqPos.x - 8, sqPos.y - 6, 16, 12);
                    EditorGUI.DrawRect(r, colorValues[c.items[Mathf.Max(0, c.items.Count - 1 - si)]]);
                }
            }

            // click handling
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (Vector2.Distance(e.mousePosition, pos) <= cellDrawSize * 0.95f)
                {
                    selectedIndex = i;
                    GUI.FocusControl(null);
                    Repaint();
                }
            }
        }

        Handles.EndGUI();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private Vector2 AxialToPixel(Vector2Int hex, Vector2 center, float size)
    {
        // pointy-top axial to pixel (2D screen coords)
        float x = size * (Mathf.Sqrt(3) * hex.x + Mathf.Sqrt(3) / 2f * hex.y);
        float y = size * (3f / 2f * hex.y);
        return center + new Vector2(x, -y); // invert y so positive r goes up visually
    }

    private void DrawColorRect(Color c, float w, float h)
    {
        Rect r = GUILayoutUtility.GetRect(w, h);
        EditorGUI.DrawRect(r, c);
    }

    private void CreateNewLevelAsset()
    {
        string path = EditorUtility.SaveFilePanelInProject("Create HexLevel asset", "NewHexLevel", "asset", "Create new HexLevel asset");
        if (string.IsNullOrEmpty(path)) return;

        var newLevel = ScriptableObject.CreateInstance<HexLevel>();
        newLevel.radius = 3;
        newLevel.EnsureGridInitialized();

        // default activate center + radius 2
        foreach (var c in newLevel.cells)
        {
            int dist = HexLevel.AxialDistance(c.pos, Vector2Int.zero);
            c.active = dist <= 2;
        }

        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();
        level = newLevel;
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
    }

    private void AutoActivateRadius2()
    {
        if (level == null) return;
        foreach (var c in level.cells)
        {
            int dist = HexLevel.AxialDistance(c.pos, Vector2Int.zero);
            c.active = dist <= 2;
        }
    }
}