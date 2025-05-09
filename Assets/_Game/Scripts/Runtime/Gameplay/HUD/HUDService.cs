﻿using System;
using Cysharp.Threading.Tasks;
using Game.Runtime.CMS;
using Game.Runtime.Gameplay.Implants.Services;
using Game.Runtime.Gameplay.Inventory;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using Game.Runtime.Services.Camera;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDService : MonoBehaviour, IService, IInitializable, IDisposable
    {
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private TVPanel tvPanel;
        [SerializeField] private ImplantsHolder implantsHolder;
        [SerializeField] private ImplantsHolder lootHolder;
        [SerializeField] private WarriorUI warriorUI;
        [SerializeField] private ImplantStatsPanel implantStatsPanel;
        [SerializeField] private EnemyUI enemyUI;
        [SerializeField] private Button endTurnButton;
        [SerializeField] private GameObject endTurnButtonParent;
        [SerializeField] private GameObject disableUI;
        
        public InventoryView InventoryView => inventoryView;
        public ImplantsHolder LootHolder => lootHolder;
        public ImplantsHolder ImplantsHolder => implantsHolder;
        public WarriorUI WarriorUI => warriorUI;
        public EnemyUI EnemyUI => enemyUI;
        public Button EndTurnButton => endTurnButton;
        public GameObject EndTurnButtonParent => endTurnButtonParent;
        public GameObject DisableUI => disableUI;
        public TVPanel TVPanel => tvPanel;

        public void Initialize()
        {
            var canvas = gameObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = ServiceLocator.Get<CameraService>().Camera;

            Subscribe();
        }

        private void Subscribe()
        {
            endTurnButton.onClick.AddListener(OnEndTurn);
            ServiceLocator.Get<InventoryService>().Stats.OnImplantStatsRecalculated += implantStatsPanel.UpdateStats;
            ServiceLocator.Get<ImplantsGameLoop>().OnNextTurnStarted += OnNextTurnStarted;
        }

        private void OnEndTurn()
        {
            ServiceLocator.Get<BattleController>().TurnAsync().Forget();
            ServiceLocator.Get<AudioService>().Play(CMs.Audio.SFX.EndTurn);
            endTurnButton.interactable = false;
        }

        private void OnNextTurnStarted()
        {
            endTurnButton.interactable = true;
        }
        
        private void Unsubscribe()
        {
            endTurnButton.onClick.RemoveAllListeners();
            ServiceLocator.Get<InventoryService>().Stats.OnImplantStatsRecalculated -= implantStatsPanel.UpdateStats;
            ServiceLocator.Get<ImplantsGameLoop>().OnNextTurnStarted -= OnNextTurnStarted;
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}