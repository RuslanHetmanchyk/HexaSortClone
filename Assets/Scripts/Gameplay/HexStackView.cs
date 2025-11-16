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

        private Vector3 currentVelocity = Vector3.zero;
        private float followSpeed = 15f;
        private float smoothTime = 0.05f;

        void HandleDrag()
        {
            // поддержка мыши (Editor/Standalone)
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                TryBeginDrag(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryEndDrag(Input.mousePosition);
            }
#endif

            // поддержка touch (мобильные)
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);

                if (t.phase == TouchPhase.Began)
                    TryBeginDrag(t.position);
                else if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
                    TryEndDrag(t.position);
            }

            if (dragging)
            {
                Vector3 screenPos = GetCurrentPointerPosition();

                Ray ray = cam.ScreenPointToRay(screenPos);
                Vector3 worldPoint;
                if (Physics.Raycast(ray, out RaycastHit hit, 200f))
                {
                    worldPoint = hit.point;
                }
                else
                {
                    Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
                    if (plane.Raycast(ray, out float enter))
                        worldPoint = ray.GetPoint(enter);
                    else
                        worldPoint = transform.position;
                }

                Vector3 desired = worldPoint + dragOffset;
                desired.y = transform.position.y;

                // ускоренное догоняющее движение
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    desired,
                    ref currentVelocity,
                    smoothTime
                );
            }
        }

        private void TryBeginDrag(Vector3 screenPosition)
        {
            Ray ray = cam.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f))
            {
                if (hit.collider != null && hit.collider.gameObject == gameObject)
                {
                    dragging = true;
                    dragOffset = transform.position - hit.point;
                    dragOffset.y = 0f;
                }
            }
        }

        private void TryEndDrag(Vector3 screenPosition)
        {
            if (dragging)
            {
                dragging = false;
                TryDrop();
            }
        }

        private Vector3 GetCurrentPointerPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.mousePresent)
                return Input.mousePosition;
#endif

            if (Input.touchCount > 0)
                return Input.GetTouch(0).position;

            return Input.mousePosition;
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