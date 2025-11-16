using System.Collections.Generic;
using System.Linq;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Commands;
using DefaultNamespace;
using UnityEngine;

public class Stack3D : MonoBehaviour
{
    [SerializeField] public HexItemView itemPrefab;
    [SerializeField] public Collider collider;
    
    public Transform itemsRoot;

    public LayerMask hexCellLayer;
    public LayerMask cellMask;

    private Camera cam;
    private bool dragging = false;
    private Vector3 dragOffset;
    
    Vector3 startPos = Vector3.zero;
    
    public HexStack HexStack;

    public List<HexItemView> items = new();
    
    public ICommandExecutionService commandService;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (IsDraggable)
        {
            HandleDrag();
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    dragging = true;
                    dragOffset = transform.position - hit.point;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (dragging)
            {
                TryDrop();
            }
            dragging = false;
        }

        if (dragging)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 100f))
            {
                Vector3 target = hit.point + dragOffset;
                transform.position = new Vector3(target.x, transform.position.y, target.z);
            }
        }
    }

    void TryDrop()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 100f, hexCellLayer))
        {
            HexCell3D cell = hit.collider.GetComponent<HexCell3D>();

            if (cell != null && cell.CanAcceptStack())
            {
                cell.PlaceStack(this);
                SetDraggableActive(false);
                
                //LevelService.Instance.OnStackMovedToCell(HexStack, cell.GridPos);
                
                StackDropData data = new StackDropData
                {
                    HexStack = HexStack,
                    TargetCellPosition = cell.GridPos,
                };
                commandService.Execute<TryDropStackToHexCellCommand, StackDropData>(data);
                
                return;
            }
        }

        // если дропнули не по клетке — стопка не исчезает
        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.position = startPos;
    }

    
    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
    }

    
    public void Init(HexStack hexStack)
    {
        HexStack = hexStack;
        
        foreach (var item in hexStack.Items)
        {
            AddItem(item);
        }
    }

    private void AddItem(StackItem stackItem)
    {
        var it = Instantiate(itemPrefab, itemsRoot);
        it.ApplyColorById(stackItem.ColorId);

        // каждый следующий появляется выше предыдущего
        it.transform.localPosition = NextItemPosition();

        items.Add(it);
    }

    public Vector3 NextItemPosition()
    {
        return GetItemPositionByIndex(items.Count);
    }
    
    public Vector3 GetItemPositionByIndex(int index)
    {
        return new Vector3(0, index * itemPrefab.Height, 0);
    }
    
    public void Add(HexItemView hexItemView)
    {
        hexItemView.transform.SetParent(itemsRoot);
        hexItemView.transform.localPosition = NextItemPosition();

        items.Add(hexItemView);
    }
    
    public HexItemView Pop()
    {
        var stackItem = items.Last();
            
        items.RemoveAt(items.Count - 1);
        stackItem.transform.SetParent(null);
            
        return stackItem;
    }

    public bool IsDraggable => collider.enabled;
    
    public void SetDraggableActive(bool active)
    {
        collider.enabled = active;
    }

    public void SetCommandRunner(ICommandExecutionService commandExecutionService)
    {
        this.commandService = commandExecutionService;
    }
}
