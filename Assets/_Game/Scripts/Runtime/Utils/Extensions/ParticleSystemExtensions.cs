using UnityEngine;

namespace Game.Runtime.Utils.Extensions
{
    public static class ParticleSystemExtensions
    {
        public static void SetEmission(this ParticleSystem particleSystem, bool enabled)
        {
            var emission = particleSystem.emission;
            emission.enabled = enabled;
        }
    }
}