using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Implants.Services;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Gameplay.Implants
{
    public class ImplantDragHandler
    {
        private readonly ImplantBehaviour _implant;
        private readonly InventoryService _inventoryService;
        
        private Vector2Int _lastSelectedSlotPosition;

        public ImplantDragHandler(ImplantBehaviour implant)
        {
            _implant = implant;
            _inventoryService = ServiceLocator.Get<InventoryService>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_inventoryService.IsBlocked) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;

            _implant.CurrentTweenScale?.Kill();
            ServiceLocator.Get<InputService>().OnRotateItem += HandleRotation;

            _implant.StartDragging();
            UpdateImplantPosition();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_implant == null) return;
            if (!_implant.IsDragging) return;
            
            UpdateImplantPosition();
            UpdateSlotHighlight();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ServiceLocator.Get<InputService>().OnRotateItem -= HandleRotation;
            if (_implant == null)  return;
            if (!_implant.IsDragging) return;

            _implant.StopDragging();
            _implant.Highlighter.HideHighlightImplant();
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.SFXImplantPut);

            if (TryPlaceItem()) return;
            if (TryReturnToHolder(eventData.position)) return;

            _implant.ReturnToOriginalPosition();
        }

        private void HandleRotation()
        {
            _implant.CurrentRotation = (_implant.CurrentRotation + 1) % 4;
            _implant.RectTransform.localRotation = Quaternion.Euler(0, 0, ImplantHelper.PresetAngles[_implant.CurrentRotation]);
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.ImplantRotate);
            UpdateImplantPosition();
            UpdateSlotHighlight();
        }

        private void UpdateImplantPosition()
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_implant.HUDRoot.transform as RectTransform, Input.mousePosition, ServiceLocator.Get<CameraService>().Camera, out var localPoint))
            {
                _implant.RectTransform.anchoredPosition = localPoint - CalculatePivotOffset();
            }
        }

        private void UpdateSlotHighlight()
        {
            if (ServiceLocator.Get<BattleController>().IsTurnStarted) return;
            
            var newSlot = GetSlotUnderCursor();
            if (newSlot == null)
            {
                ClearHighlights();
                _implant.Highlighter.HideHighlightImplant();
                return;
            }

            if (_lastSelectedSlotPosition != newSlot.GridPosition)
                ClearHighlights();
            
            _lastSelectedSlotPosition = newSlot.GridPosition;
            if (!_inventoryService.Highlighter.TryHighlightSynergySlots(_implant, newSlot))
                ClearHighlights();

            if (_inventoryService.CanPlaceItem(_implant, newSlot.GridPosition, _implant.CurrentRotation))
                _inventoryService.SetItemPosition(newSlot, _implant, true);
            else _implant.Highlighter.HideHighlightImplant();
        }

        private void ClearHighlights()
        {
            _inventoryService.Highlighter.ResetSlotsHighlight();
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

        private bool TryPlaceItem()
        {
            var slot = GetSlotUnderCursor();
            if (slot == null) return false;

            _implant.CenterSlotPosition = slot.GridPosition;
            if (!ServiceLocator.Get<InventoryService>().TryPlaceItem(_implant, _implant.CenterSlotPosition))
                return false;
            
            if (ServiceLocator.Get<ImplantsHolderService>().HasItem(_implant))
                ServiceLocator.Get<ImplantsHolderService>().RemoveItem(_implant);
            
            ServiceLocator.Get<InventoryService>().SetItemPosition(slot, _implant, false);
            return true;
        }

        private bool TryReturnToHolder(Vector2 position)
        {
            var result = ServiceLocator.Get<ImplantsHolderService>().TryReturnToHolder(_implant, position);
            if (!result) return false;
            
            if (ServiceLocator.Get<InventoryService>().HasItem(_implant))
                ServiceLocator.Get<InventoryService>().RemoveItem(_implant);
            
            ServiceLocator.Get<LootService>().Choice(_implant.Model.EntityId);
            _implant.CurrentRotation = 0;
            _implant.RectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            return true;
        }
        
        private Vector2 CalculatePivotOffset()
        {
            Vector2 pivotDifference = new Vector2(
                _implant.PivotPoint.x - 0.5f,
                _implant.PivotPoint.y - 0.5f);

            return ImplantHelper.ApplyRotationToOffset(
                new Vector2(
                    pivotDifference.x * _implant.RectTransform.rect.width,
                    pivotDifference.y * _implant.RectTransform.rect.height),
                _implant.CurrentRotation);
        }
    }
}