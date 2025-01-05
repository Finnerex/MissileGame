using System;
using UnityEngine;

namespace Aerodynamics
{
    public class AoaEffect : MonoBehaviour
    {
        [SerializeField] private AeroSurface referenceSurface;
        [SerializeField] private float aoaThreshold = 20;
        [SerializeField] private TrailRenderer effect;

        private void Update()
        {
            bool aboveThreshold = Mathf.Abs(referenceSurface.aoa) > aoaThreshold;
            
            if (!effect.emitting && aboveThreshold)
                effect.emitting = true;
            else if (effect.emitting && !aboveThreshold)
                effect.emitting = false;
        }
    }
}