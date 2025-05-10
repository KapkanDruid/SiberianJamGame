using System;
using System.Collections.Generic;
using Game.Runtime.Gameplay.Implants;
using UnityEngine;

namespace Game.Runtime.Gameplay.HUD
{
    [Serializable]
    public class TVPanel
    {
        [SerializeField] private RectTransform firstGroup;
        [SerializeField] private RectTransform secondGroup;

        public void SetImplants(List<ImplantBehaviour> implants)
        {
            for (int i = 0; i < implants.Count; i++)
            {
                RectTransform targetGroup = i < 2 ? firstGroup : secondGroup;
                SetGroup(targetGroup, implants[i]);
            }
        }

        private void SetGroup(RectTransform group, ImplantBehaviour item)
        {
            item.transform.SetParent(group);
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            item.transform.localScale = Vector3.one * 0.7f;
        }
    }
}