using System;
using Game.Runtime.Gameplay.Implants;
using UnityEngine;

namespace Game.Runtime.CMS.Components.Implants
{
    public class ImplantSynergyColorsComponent : CMSComponent
    {
        public ImplantSynergyColor[] SynergyColors;
    }

    [Serializable]
    public class ImplantSynergyColor
    {
        public ImplantType ImplantType;
        public Color Color;
    }
}