using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Implants
{
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
    public class ImplantBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform highlightImplant;
        
        private ImplantPointerHandler _implantPointerHandler;
        private ImplantDragHandler _implantDragHandler;
        
        private Transform _originalParent;
        private Vector2 _originalPosition;
        private int _originIndex;
        private int _originalRotation;
        
        private InventoryService _inventoryService;
        private AudioService _audioService;
        
        private ParticleSystem _particleSystem;
        public Tweener CurrentTweenScale;
        
        public int CurrentRotation { get;  set; }
        public Vector2Int CenterSlotPosition { get; set; }
        
        public CMSEntity Model { get; private set; }
        public RectTransform RectTransform => _rectTransform;
        public RectTransform HUDRoot { get; private set; }
        public List<Vector2Int> SlotPositions { get; private set; }
        public Vector3 OriginalScale { get; private set; }
        public Vector2 PivotPoint { get; private set; }
        public bool IsDragging { get; private set; }

        public RectTransform HighlightImplant => highlightImplant;

        public void SetupItem(CMSEntity itemModel, RectTransform hudRoot)
        {
            Model = itemModel;
            HUDRoot = hudRoot;
            
            _implantPointerHandler = new ImplantPointerHandler(this);
            _implantDragHandler = new ImplantDragHandler(this);
            
            _inventoryService = ServiceLocator.Get<InventoryService>();
            _audioService = ServiceLocator.Get<AudioService>();

            var itemComponent = itemModel.GetComponent<InventoryItemComponent>();
            PivotPoint = itemComponent.Pivot;
            SlotPositions = itemComponent.Grid.GridPattern;
            _rectTransform.sizeDelta = itemComponent.SizeDelta;
            HighlightImplant.sizeDelta = itemComponent.SizeDelta;
            _image.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;

            var highlightImage = HighlightImplant.GetComponent<Image>();
            highlightImage.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;
            
            OriginalScale = transform.localScale;
            
            var particlePrefab = itemComponent.Particle;
            _particleSystem = Instantiate(particlePrefab, transform);
            _particleSystem.Stop();
            
            HighlightImplant.gameObject.SetActive(false);
        }

        public void PlayParticle() => _particleSystem.Play();

        public ImplantType GetImplantType()
        {
            if (Model.Is<HealthImplantComponent>()) return ImplantType.Health;
            if (Model.Is<ArmorImplantComponent>()) return ImplantType.Armor;
            return ImplantType.Damage;
        }

        public void StartDragging()
        {
            IsDragging = true;

            _originalParent = transform.parent;
            _originalPosition = _rectTransform.anchoredPosition;
            _originalRotation = CurrentRotation;
            _canvasGroup.blocksRaycasts = false;
            _originIndex = transform.GetSiblingIndex();

            transform.SetParent(HUDRoot.transform);
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

            transform.localScale = OriginalScale * 1.2f;
            _audioService.Play(CMs.Audio.SFX.SFXImplantDrag);
        }

        public void StopDragging()
        {
            IsDragging = true;
            _canvasGroup.blocksRaycasts = true;
            transform.DOScale(OriginalScale, 0.1f);
            _inventoryService.SynergySlots.Clear();
            _inventoryService.Highlighter.ResetSlotHighlight(this);
        }

        public void ReturnToOriginalPosition()
        {
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _originalRotation);
            CurrentRotation = _originalRotation;
            _inventoryService.Highlighter.UpdateAllSlotVisual();
            transform.SetSiblingIndex(_originIndex);
        }

        public void OnPointerEnter(PointerEventData eventData) => _implantPointerHandler.OnPointerEnter(eventData);
        public void OnPointerExit(PointerEventData eventData) => _implantPointerHandler.OnPointerExit(eventData);
        public void OnPointerDown(PointerEventData eventData) => _implantDragHandler.OnPointerDown(eventData);
        public void OnDrag(PointerEventData eventData) => _implantDragHandler.OnDrag(eventData);
        public void OnPointerUp(PointerEventData eventData) => _implantDragHandler.OnPointerUp(eventData);

        private void OnDestroy() => CurrentTweenScale?.Kill();
    }
}