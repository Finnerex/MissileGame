using System;
using UnityEngine;

namespace Aerodynamics
{
    public class Thruster : MonoBehaviour
    {

        [SerializeField] private Rigidbody rb;
        
        // not actually a hard limit just used for the animation curve
        [SerializeField] private float maxSpeedMpS = 600;
        [SerializeField] private float maxAltM = 5000;
        
        [SerializeField] private float thrustForceMultiplier = 5f;
        [SerializeField] private AnimationCurve thrustThrottleCurve;
        [SerializeField] private AnimationCurve thrustVelocityCurve;
        [SerializeField] private AnimationCurve thrustAltitudeCurve;
        
        private Transform _transform;
        private float _throttle;
        private float _inverseMaxSpeed;
        private float _inverseMaxAlt;
        
        public float Throttle
        {
            get => _throttle;
            set => _throttle = Mathf.Clamp(value, 0, 1);
        }
        
        private void Start()
        {
            _transform = transform;
            _inverseMaxSpeed = 1 / maxSpeedMpS;
            _inverseMaxAlt = 1 / maxAltM;
        }
        
        private void FixedUpdate()
        {
            Vector3 thrustForce = Throttle * thrustForceMultiplier *
                                  thrustThrottleCurve.Evaluate(Throttle) *
                                  thrustVelocityCurve.Evaluate(_inverseMaxSpeed * rb.velocity.magnitude) *
                                  thrustAltitudeCurve.Evaluate(_inverseMaxAlt * _transform.position.y) *
                                  _transform.forward;

            rb.AddForce(thrustForce);
        }
    }
}