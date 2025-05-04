using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using Game.Runtime.Services.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Implants
{
    public enum ImplantType
    {
        Health,
        Armor,
        Damage
    }
    
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
    public class ImplantBehaviour : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;

        public List<Vector2Int> SlotPositions { get; private set; }
        public CMSEntity Model { get; private set; }
        public int CurrentRotation { get; private set; }
        public Vector2Int CenterSlotPosition { get; private set; }

        private Transform _originalParent;
        private Vector2 _originalPosition;
        private int _originIndex;
        private InventoryService _inventoryService;
        private ImplantsHolderService _holderService;
        private Canvas _root;
        private bool _isDragging;
        private int _originalRotation;
        private Vector2 _pivotPoint;
        
        private Vector3 _originalScale;
        private Tweener _currentTweenScale;
        
        private ParticleSystem _particleSystem;

        private void Start()
        {
            transform.localScale = Vector3.one;
        }

        public void SetupItem(CMSEntity itemModel, Canvas root)
        {
            Model = itemModel;
            _root = root;
            
            _inventoryService = SL.Get<InventoryService>();
            _holderService = SL.Get<ImplantsHolderService>();

            var itemComponent = itemModel.GetComponent<InventoryItemComponent>();
            _pivotPoint = itemComponent.Pivot;
            SlotPositions = itemComponent.Grid.GridPattern;
            _rectTransform.sizeDelta = itemComponent.SizeDelta;
            _image.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;
            
            _originalScale = _rectTransform.localScale;
            
            var particlePrefab = itemComponent.Particle;
            _particleSystem = Instantiate(particlePrefab, transform);
            _particleSystem.Stop();
        }

        public void PlayParticle()
        {
            _particleSystem.Play();
        }

        public ImplantType GetImplantType()
        {
            if (Model.Is<HealthImplantComponent>()) return ImplantType.Health;
            if (Model.Is<ArmorImplantComponent>()) return ImplantType.Armor;
            return ImplantType.Damage;
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this == null)return;
            if (_isDragging) return;
            if (SL.Get<InventoryService>().HasItem(this)) return;
            
            _currentTweenScale?.Kill();
            _currentTweenScale = _rectTransform.DOScale(_originalScale * 1.3f, 0.2f)
                .SetEase(Ease.OutBack).OnKill(() => _currentTweenScale = null);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (this == null)return;
            if (_isDragging) return;
            if (SL.Get<InventoryService>().HasItem(this)) return;

            _currentTweenScale?.Kill();
            _currentTweenScale = _rectTransform.DOScale(_originalScale, 0.2f)
                .SetEase(Ease.InOutQuad).OnKill(() => _currentTweenScale = null);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (this == null)return;
            if (_isDragging) return;
            
            _currentTweenScale?.Kill();

            if (eventData.button != PointerEventData.InputButton.Left) return;
            SL.Get<AudioService>().Play(CMs.Audio.SFX.SFXImplantDrag);
            StartDragging();
            SL.Get<InputService>().OnRotateItem += HandleRotation;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this == null)return;
            if (!_isDragging) return;
            UpdateDragPosition(eventData);
            UpdateSlotHighlight();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (this == null)
            {
                SL.Get<InputService>().OnRotateItem -= HandleRotation;
                return;
            }
            if (!_isDragging) return;

            StopDragging();

            if (TryPlaceItem()) return;
            if (TryReturnToHolder(eventData.position)) return;

            ReturnToOriginalPosition();
        }

        public void PingPongScale()
        {
            transform.DOScale(Vector3.one * 1.2f, 0.1f).SetLoops(2, LoopType.Yoyo);
           //PlayParticle();
        }

        private void StartDragging()
        {
            _isDragging = true;
            _originalParent = transform.parent;
            _originalPosition = _rectTransform.anchoredPosition;
            _originalRotation = CurrentRotation;
            _canvasGroup.blocksRaycasts = false;
            _originIndex = transform.GetSiblingIndex();

            transform.SetParent(_root.transform);
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            transform.localScale = Vector3.one * 1.2f;
        }

        private void StopDragging()
        {
            _isDragging = false;
            _canvasGroup.blocksRaycasts = true;
            SL.Get<InputService>().OnRotateItem -= HandleRotation;
            transform.DOScale(Vector3.one, 0.1f);
            ResetHighlight();
        }

        private Vector2 _lastPosition;
        private void UpdateDragPosition(PointerEventData eventData)
        {
            if (TryGetLocalPoint(eventData, out Vector2 localPoint))
            {
                var targetPosition = localPoint - CalculatePivotOffset();
                _rectTransform.anchoredPosition = _lastPosition = targetPosition;
            }
        }

        private bool TryGetLocalPoint(PointerEventData eventData, out Vector2 localPoint)
        {
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(_root.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        }

        private Vector2 CalculatePivotOffset()
        {
            Vector2 pivotDifference = new Vector2(
                _pivotPoint.x - 0.5f,
                _pivotPoint.y - 0.5f);

            return ImplantHelper.ApplyRotationToOffset(
                new Vector2(
                    pivotDifference.x * _rectTransform.rect.width,
                    pivotDifference.y * _rectTransform.rect.height),
                CurrentRotation);
        }

        private void UpdateSlotHighlight()
        {
            if (SL.Get<BattleController>().IsTurnStarted) return;
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

            if (_holderService.HasItem(this))
                _holderService.RemoveItem(this);
            
            SL.Get<AudioService>().Play(CMs.Audio.SFX.SFXImplantPut);

            _inventoryService.SetItemPosition(slot, this);
            CenterSlotPosition = slot.GridPosition;
            //PlayParticle();
            return true;
        }

        private bool TryReturnToHolder(Vector2 position)
        {
            var result = _holderService.TryReturnToHolder(this, position);
            if (!result) return false;
            
            if (_inventoryService.HasItem(this))
                _inventoryService.RemoveItem(this);
            
            SL.Get<LootService>().Choice(Model.EntityId);

            CurrentRotation = 0;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            return true;
        }
        
        private void HandleRotation()
        {
            if (!_isDragging) return;

            RotateItem();
        }

        private void RotateItem()
        {
            if (this == null)
            {
                SL.Get<InputService>().OnRotateItem -= HandleRotation;
                return;
            }
            CurrentRotation = (CurrentRotation + 1) % 4;
            float[] presetAngles = { 0f, -90f, -180f, -270f };
            _rectTransform.localRotation = Quaternion.Euler(0, 0, presetAngles[CurrentRotation]);
            _rectTransform.anchoredPosition = _lastPosition;
            UpdateSlotHighlight();
        }

        private void ReturnToOriginalPosition()
        {
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _originalRotation);
            CurrentRotation = _originalRotation;
            _inventoryService.UpdateAllSlotVisual();
            transform.SetSiblingIndex(_originIndex);
        }

        private void OnDestroy()
        {
            _currentTweenScale?.Kill();
        }
    }
}