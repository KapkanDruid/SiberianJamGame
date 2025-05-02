using System.Collections.Generic;
using Game.Runtime.Gameplay.HUD;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    public Vector2Int ItemSize { get; private set; }

    private List<Vector2Int> _slotPositions = new List<Vector2Int>();
    private Transform _originalParent;
    private Vector2 _originalPosition;
    private InventoryService _inventoryService;
    private int _currentRotation = 0;
    private bool _isDragging = false;
    private Vector2 _dragOffset;

    public List<Vector2Int> SlotPositions => _slotPositions;
    public int CurrentRotation => _currentRotation;
    public Sprite ItemSprite => image.sprite;

    public void Configure(List<Vector2Int> slotPositions, Sprite sprite, Vector2 sizeDelta)
    {
        _slotPositions = slotPositions;
        image.sprite = sprite;
        rectTransform.sizeDelta = sizeDelta;
        _inventoryService = SL.Get<InventoryService>();
        ItemSize = CalculateSize();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Вращение при начале drag правой кнопкой
            RotateItem();
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left) return;

        _isDragging = true;
        _originalParent = transform.parent;
        _originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    private InventorySlot _lastSlotUnderCursor;
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
         
            var slotUnderCursor = GetSlotUnderCursor();
            if (slotUnderCursor != null)
            {
                if (_lastSlotUnderCursor != null) _lastSlotUnderCursor.SetHighlight(false);
                _lastSlotUnderCursor = slotUnderCursor;
                _inventoryService.UpdateSlotHighlight(this, slotUnderCursor.GridPosition);
            }
            else
            {
                if (_lastSlotUnderCursor != null) _lastSlotUnderCursor.SetHighlight(false);
            }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        canvasGroup.blocksRaycasts = true;

        // Проверяем, был ли предмет помещен в инвентарь

        var inventorySlot = GetSlotUnderCursor();
        if (inventorySlot != null && _inventoryService.TryPlaceItem(this, inventorySlot.GridPosition))
        {
            _inventoryService.SetItemPosition(inventorySlot, this);
        }
        else
        {
            if (_inventoryService.HasItem(this) && _inventoryService.IsOutsideInventory(eventData.position))
            {
                _inventoryService.RemoveItem(this);
                transform.SetParent(GetComponentInParent<Canvas>().transform);
                rectTransform.position = eventData.position;
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
    }

    private InventorySlot GetSlotUnderCursor()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
    
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
    
        foreach (var result in results)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();
            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }

    private void RotateItem()
    {
        _currentRotation = (_currentRotation + 1) % 4;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _currentRotation);
    }

    private Vector2Int CalculateSize()
    {
        if (_slotPositions.Count == 0) return Vector2Int.zero;

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;

        foreach (var pos in _slotPositions)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    // private void PlaceItemInSlot(Transform slotTransform)
    // {
    //     transform.SetParent(slotTransform);
    //     rectTransform.anchoredPosition = Vector2.zero;
    //     rectTransform.localRotation = Quaternion.identity;
    //     _currentRotation = 0;
    // }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        rectTransform.anchoredPosition = _originalPosition;
        rectTransform.localRotation = Quaternion.identity;
        _currentRotation = 0;

        if (_originalParent != null && _originalParent.TryGetComponent<InventorySlot>(out var slot))
        {
            _inventoryService.TryPlaceItem(this, slot.GridPosition);
        }
    }
}