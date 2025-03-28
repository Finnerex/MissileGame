using System;
using UnityEngine;

namespace Aerodynamics
{
    public class AoaEffect : MonoBehaviour
    {
        [SerializeField] private AeroSurface referenceSurface;
        [SerializeField] private float aoaThreshold = 20;
        [SerializeField] private Component effect;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Vector2 speedRange;
        
        private bool _emitting;
        private Vector2 _sqrSpeedRange;

        private void Start()
        {
            _sqrSpeedRange.x = speedRange.x * speedRange.x;
            _sqrSpeedRange.y = speedRange.y * speedRange.y;
        }

        private void Update()
        {
            bool aboveThreshold = Mathf.Abs(referenceSurface.aoa) > aoaThreshold;

            if (!_emitting && aboveThreshold &&
                rb.velocity.sqrMagnitude > _sqrSpeedRange.x && rb.velocity.sqrMagnitude < _sqrSpeedRange.y)
            {
                if (effect is TrailRenderer trail)
                    trail.emitting = true;
                else if (effect is ParticleSystem ps)
                    ps.Play();

                _emitting = true;
            }
            else if (_emitting && (!aboveThreshold ||
                     rb.velocity.sqrMagnitude <= _sqrSpeedRange.x || rb.velocity.sqrMagnitude >= _sqrSpeedRange.y))
            {
                if (effect is TrailRenderer trail)
                    trail.emitting = false;
                else if (effect is ParticleSystem ps)
                    ps.Stop();

                _emitting = false;
            }
            
        }
    }
}