using System.Collections.Generic;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Services;
using Game.Runtime.Services.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    public List<Vector2Int> SlotPositions { get; private set; }

    private Transform _originalParent;
    private Vector2 _originalPosition;
    private InventoryService _inventoryService;
    private Canvas _canvas;
    private bool _isDragging;
    private int _currentRotation;
    private int _originalRotation;

    public int CurrentRotation => _currentRotation;

    public void SetupItem(CMSEntity itemModel)
    {
        _inventoryService = SL.Get<InventoryService>();
        _canvas = GetComponentInParent<Canvas>();

        var itemComponent = itemModel.GetComponent<InventoryItemComponent>();
        SlotPositions = itemComponent.Grid.GridPattern;
        rectTransform.sizeDelta = itemComponent.SizeDelta;
        image.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;
    }

    private void HandleRotateItem()
    {
        if (_isDragging)
        {
            RotateItem();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;

        _originalParent = transform.parent;
        _originalPosition = rectTransform.anchoredPosition;
        _originalRotation = _currentRotation;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(GetComponentInParent<Canvas>().transform);
        
        SL.Get<InputService>().OnRotateItem += HandleRotateItem;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        
        rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
         
        var slotUnderCursor = GetSlotUnderCursor();
        if (slotUnderCursor != null)
        {
            ResetColorLastSlotUnderCursor();
            _inventoryService.UpdateSlotHighlight(this, slotUnderCursor.GridPosition, Color.yellow);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;
        
        canvasGroup.blocksRaycasts = true;

        var inventorySlot = GetSlotUnderCursor();
        if (inventorySlot != null && _inventoryService.TryPlaceItem(this, inventorySlot.GridPosition))
        {
            _inventoryService.SetItemPosition(inventorySlot, this);
        }
        else
        {
            if (_inventoryService.NeedRemoveItem(this, eventData.position))
            {
                ResetColorLastSlotUnderCursor();
                _inventoryService.RemoveItem(this);
                transform.SetParent(_canvas.transform);
                rectTransform.position = eventData.position;
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
        
        SL.Get<InputService>().OnRotateItem -= HandleRotateItem;
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

    private void ResetColorLastSlotUnderCursor()
    {
        _inventoryService.ResetSlotHighlight(this);
    }
    
    private void RotateItem()
    {
        _currentRotation = (_currentRotation + 1) % 4;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _currentRotation);
        
        var slotUnderCursor = GetSlotUnderCursor();
        if (slotUnderCursor != null)
        {
            ResetColorLastSlotUnderCursor();
            _inventoryService.UpdateSlotHighlight(this, slotUnderCursor.GridPosition, Color.yellow);
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        rectTransform.anchoredPosition = _originalPosition;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _originalRotation);;
        _currentRotation = _originalRotation;
        _inventoryService.UpdateAllSlotVisual();
    }
}