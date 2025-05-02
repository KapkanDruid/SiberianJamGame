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
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;

    public List<Vector2Int> SlotPositions { get; private set; }
    public int CurrentRotation { get; private set; }

    private Transform _originalParent;
    private Vector2 _originalPosition;
    private InventoryService _inventoryService;
    private Canvas _canvas;
    private bool _isDragging;
    private int _originalRotation;
    private Vector2 _pivotPoint;

    private enum EndDragState { None, Placing, Removing }

    public void SetupItem(CMSEntity itemModel)
    {
        _inventoryService = SL.Get<InventoryService>();
        _canvas = GetComponentInParent<Canvas>();

        var itemComponent = itemModel.GetComponent<InventoryItemComponent>();
        _pivotPoint = itemComponent.Pivot;
        SlotPositions = itemComponent.Grid.GridPattern;
        _rectTransform.sizeDelta = itemComponent.SizeDelta;
        _image.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StartDragging();
        SL.Get<InputService>().OnRotateItem += HandleRotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        UpdateDragPosition(eventData);
        UpdateSlotHighlight();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        StopDragging();

        switch (DetermineDragResult(eventData))
        {
            case EndDragState.Placing:
                TryPlaceItem();
                break;
            case EndDragState.Removing:
                TryRemoveItem(eventData.position);
                break;
            default:
                ReturnToOriginalPosition();
                break;
        }
    }

    private EndDragState DetermineDragResult(PointerEventData eventData)
    {
        if (TryPlaceItem()) return EndDragState.Placing;
        if (TryRemoveItem(eventData.position)) return EndDragState.Removing;
        return EndDragState.None;
    }

    private void StartDragging()
    {
        _isDragging = true;
        _originalParent = transform.parent;
        _originalPosition = _rectTransform.anchoredPosition;
        _originalRotation = CurrentRotation;
        _canvasGroup.blocksRaycasts = false;

        transform.SetParent(_canvas.transform);
    }

    private void StopDragging()
    {
        _isDragging = false;
        _canvasGroup.blocksRaycasts = true;
        SL.Get<InputService>().OnRotateItem -= HandleRotation;
        ResetHighlight();
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (TryGetLocalPoint(eventData, out Vector2 localPoint))
        {
            _rectTransform.anchoredPosition = localPoint - CalculatePivotOffset(eventData);
        }
    }

    private bool TryGetLocalPoint(PointerEventData eventData, out Vector2 localPoint)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);
    }

    private Vector2 CalculatePivotOffset(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 _);

        Vector2 pivotDifference = new Vector2(
            _pivotPoint.x - 0.5f,
            _pivotPoint.y - 0.5f);

        return Helpers.InventoryHelper.ApplyRotationToOffset(
            new Vector2(
                pivotDifference.x * _rectTransform.rect.width,
                pivotDifference.y * _rectTransform.rect.height),
            CurrentRotation);
    }

    private void UpdateSlotHighlight()
    {
        var newSlot = GetSlotUnderCursor();
        if (newSlot != null)
        {
            ResetHighlight();
            _inventoryService.UpdateSlotHighlight(this, newSlot.GridPosition, Color.yellow);
        }
    }

    private InventorySlot GetSlotUnderCursor()
    {
        var pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent(out InventorySlot slot))
            {
                return slot;
            }
        }

        return null;
    }

    private void ResetHighlight()
    {
        _inventoryService.ResetSlotHighlight(this);
    }

    private bool TryPlaceItem()
    {
        var slot = GetSlotUnderCursor();
        if (slot == null || !_inventoryService.TryPlaceItem(this, slot.GridPosition))
            return false;

        _inventoryService.SetItemPosition(slot, this);
        return true;
    }

    private bool TryRemoveItem(Vector2 position)
    {
        if (!_inventoryService.NeedRemoveItem(this, position))
            return false;

        _inventoryService.RemoveItem(this);
        transform.SetParent(_canvas.transform);
        _rectTransform.position = position;
        return true;
    }

    private void HandleRotation()
    {
        if (!_isDragging) return;

        RotateItem();
    }

    private void RotateItem()
    {
        CurrentRotation = (CurrentRotation + 1) % 4;
        _rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * CurrentRotation);

        UpdateDragPosition(new PointerEventData(EventSystem.current) { position = Input.mousePosition });
        UpdateSlotHighlight();
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;
        _rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _originalRotation);
        CurrentRotation = _originalRotation;
        _inventoryService.UpdateAllSlotVisual();
    }
}