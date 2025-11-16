using System.Collections.Generic;
using System.Linq;
using Core.Services.CommandRunner.Interfaces;
using Core.Services.Gameplay.Commands;
using Core.Services.Gameplay.Level.Implementation;
using UnityEngine;

namespace Gameplay
{
    public class HexStackView : MonoBehaviour
    {
        [SerializeField] private HexItemView itemPrefab;
        [SerializeField] private Collider collider;

        [SerializeField] private LayerMask hexCellLayer;

        private Camera cam;
        private bool dragging = false;
        private Vector3 dragOffset;

        private Vector3 startPos = Vector3.zero;

        private HexStack HexStack;

        private readonly List<HexItemView> itemViews = new();

        private ICommandExecutionService commandService;

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
                HexCellView cell = hit.collider.GetComponent<HexCellView>();

                if (cell != null && cell.CanAcceptStack())
                {
                    cell.PlaceStack(this);
                    SetDraggableActive(false);

                    var data = new StackDropData
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

        private void AddItem(HexItem hexItem)
        {
            var it = Instantiate(itemPrefab, transform);
            it.ApplyColorById(hexItem.ColorId);

            // каждый следующий появляется выше предыдущего
            it.transform.localPosition = NextItemPosition();

            itemViews.Add(it);
        }

        public Vector3 NextItemPosition()
        {
            return GetItemPositionByIndex(itemViews.Count);
        }

        public Vector3 GetItemPositionByIndex(int index)
        {
            return new Vector3(0, index * itemPrefab.Height, 0);
        }

        public void Add(HexItemView hexItemView)
        {
            hexItemView.transform.SetParent(transform);
            hexItemView.transform.localPosition = NextItemPosition();

            itemViews.Add(hexItemView);
        }

        public HexItemView Pop()
        {
            var stackItem = itemViews.Last();

            itemViews.RemoveAt(itemViews.Count - 1);
            stackItem.transform.SetParent(null);

            return stackItem;
        }

        public void Push(HexItemView hexItemView)
        {
            itemViews.Add(hexItemView);
            hexItemView.transform.SetParent(transform);
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
}