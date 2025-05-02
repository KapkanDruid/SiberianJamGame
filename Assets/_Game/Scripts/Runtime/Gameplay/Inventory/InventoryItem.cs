using System.Collections.Generic;
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
        UpdateItemSize();
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

        // Вычисляем offset между позицией курсора и центром предмета
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);
        _dragOffset = rectTransform.anchoredPosition - localPoint;

        _inventoryService.RemoveItem(this);
        transform.SetParent(GetComponentInParent<Canvas>().transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Вращение при drag правой кнопкой
            RotateItem();
        }

        // Перемещение при drag левой кнопкой
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint + _dragOffset;
            UpdateSlotHighlight(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        _isDragging = false;

        canvasGroup.blocksRaycasts = true;

        // Проверяем, был ли предмет помещен в инвентарь
        var inventorySlot = eventData.pointerCurrentRaycast.gameObject?.GetComponent<InventorySlot>();
        if (inventorySlot != null && _inventoryService.TryPlaceItem(this, inventorySlot.GridPosition))
        {
            PlaceItemInSlot(inventorySlot.transform);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void UpdateSlotHighlight(PointerEventData eventData)
    {
        // Получаем все слоты инвентаря
        InventorySlot[] allSlots = FindObjectsOfType<InventorySlot>();

        // Получаем позицию нашего предмета в пространстве UI
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint);

        // Находим ближайший слот к позиции курсора
        InventorySlot closestSlot = null;
        float closestDistance = float.MaxValue;

        foreach (var slot in allSlots)
        {
            // Преобразуем позицию слота в локальные координаты того же родителя
            Vector2 slotScreenPos = RectTransformUtility.WorldToScreenPoint(null, slot.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                slotScreenPos,
                null,
                out Vector2 slotLocalPos
            );

            // Рассчитываем расстояние
            float distance = Vector2.Distance(localPoint, slotLocalPos);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        // Сбрасываем подсветку всех слотов
        foreach (var slot in allSlots)
        {
            slot.SetHighlight(false);
        }

        // Если нашли ближайший слот, подсвечиваем все слоты, которые займет предмет
        if (closestSlot != null)
        {
            Vector2Int gridPos = closestSlot.GridPosition;

            // Проверяем, можно ли разместить предмет в этой позиции
            bool canPlace = _inventoryService.CanPlaceItem(this, gridPos);

            // Подсвечиваем все слоты, которые займет предмет
            for (int x = 0; x < CalculateSize().x; x++)
            {
                for (int y = 0; y < CalculateSize().y; y++)
                {
                    int checkX = gridPos.x + x;
                    int checkY = gridPos.y + y;

                    // Находим соответствующий слот
                    InventorySlot targetSlot = _inventoryService.FindSlotAtPosition(new Vector2Int(checkX, checkY));
                    if (targetSlot != null)
                    {
                        // Подсвечиваем красным, если нельзя разместить, желтым - если можно
                        targetSlot.SetHighlight(canPlace);
                    }
                }
            }
        }
    }


    private void RotateItem()
    {
        _currentRotation = (_currentRotation + 1) % 4;
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _currentRotation);
        UpdateItemSize();
    }

    private void UpdateItemSize()
    {
        var size = CalculateSize();
        rectTransform.sizeDelta = size * _inventoryService.CellSize;
    }

    public Vector2Int CalculateSize()
    {
        if (_slotPositions.Count == 0) return Vector2Int.zero;

        int minX = _slotPositions[0].x;
        int maxX = _slotPositions[0].x;
        int minY = _slotPositions[0].y;
        int maxY = _slotPositions[0].y;

        foreach (var pos in _slotPositions)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    private void PlaceItemInSlot(Transform slotTransform)
    {
        transform.SetParent(slotTransform);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        _currentRotation = 0;
    }

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