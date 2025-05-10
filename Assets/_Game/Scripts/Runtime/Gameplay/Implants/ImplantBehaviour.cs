using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Runtime.CMS;
using Game.Runtime.CMS.Components.Commons;
using Game.Runtime.CMS.Components.Gameplay;
using Game.Runtime.CMS.Components.Implants;
using Game.Runtime.Gameplay.Implants.Services;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.Implants
{
    [RequireComponent(typeof(RectTransform), typeof(Image), typeof(CanvasGroup))]
    public class ImplantBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _image;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private ImplantHighlighter implantHighlighter;
        
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
        public bool CanInteract { get; set; }

        public CMSEntity Model { get; private set; }
        public RectTransform RectTransform => _rectTransform;
        public RectTransform HUDRoot { get; private set; }
        public List<Vector2Int> SlotPositions { get; private set; }
        public Vector3 OriginalScale { get; private set; }
        public Vector2 PivotPoint { get; private set; }
        public bool IsLocalDragging { get; private set; }

        public ImplantHighlighter Highlighter => implantHighlighter;

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
            _image.sprite = itemModel.GetComponent<SpriteComponent>().Sprite;

            OriginalScale = transform.localScale;
            
            var particlePrefab = itemComponent.Particle;
            _particleSystem = Instantiate(particlePrefab, transform);
            _particleSystem.Stop();
            
            Highlighter.SetupHighlightImplant(itemModel, this);
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
            ServiceLocator.Get<ImplantsGameLoop>().IsGlobalDragging = true;
            IsLocalDragging = true;
            
            _originalParent = transform.parent;
            _originalPosition = _rectTransform.anchoredPosition;
            _originalRotation = CurrentRotation;
            _canvasGroup.blocksRaycasts = false;
            _originIndex = transform.GetSiblingIndex();

            transform.SetParent(HUDRoot.transform);
            SetCenterRectTransform();

            transform.localScale = OriginalScale * 1.2f;
            _audioService.Play(CMs.Audio.SFX.SFXImplantDrag);
        }

        public void SetCenterRectTransform()
        {
            _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _rectTransform.anchorMax = new Vector2(0.5f, 0.5f); 
        }

        public void StopDragging()
        {
            ServiceLocator.Get<ImplantsGameLoop>().IsGlobalDragging = false;
            IsLocalDragging = false;

            _canvasGroup.blocksRaycasts = true;
            transform.DOScale(OriginalScale, 0.1f);
            _inventoryService.Highlighter.ResetSlotsHighlight();
        }

        public void ReturnToOriginalPosition()
        {
            transform.SetParent(_originalParent);
            _rectTransform.anchoredPosition = _originalPosition;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * _originalRotation);
            CurrentRotation = _originalRotation;
            _inventoryService.Highlighter.UpdateSlotsHighlight();
            transform.SetSiblingIndex(_originIndex);
        }

        public async UniTask ReleaseImplant()
        {
            await transform.DOScale(Vector3.zero, 0.2f).ToUniTask();
            Destroy(Highlighter.HighlightImplant.gameObject);
            Destroy(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData) => _implantPointerHandler.OnPointerEnter();
        public void OnPointerExit(PointerEventData eventData) => _implantPointerHandler.OnPointerExit();
        public void OnBeginDrag(PointerEventData eventData) => _implantDragHandler.OnBeginDragHandler(eventData);
        public void OnDrag(PointerEventData eventData) => _implantDragHandler.OnDrag();
        public void OnEndDrag(PointerEventData eventData) => _implantDragHandler.OnEndDrag();

        private void OnDestroy() =>CurrentTweenScale?.Kill();
    }
}