using System;
using Game.Runtime.CMS;
using Game.Runtime.Services;
using Game.Runtime.Services.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Gameplay.HUD
{
    public class HUDBehaviour : MonoBehaviour
    {
        [SerializeField] private InventoryView inventoryView;
        [SerializeField] private ImplantsHolder implantsHolder;
        [SerializeField] private ImplantsHolder lootHolder;
        [SerializeField] private WarriorUI warriorUI;
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

        public void Start()
        {
            endTurnButton.onClick.AddListener(() => SL.Get<AudioService>().Play(CMs.Audio.SFX.EndTurn));
        }
    }
}