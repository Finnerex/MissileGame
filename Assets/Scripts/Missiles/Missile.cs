using Aerodynamics;
using Missiles.Components;
using UnityEngine;
using Utility;

namespace Missiles
{
    public class Missile : MonoBehaviour
    {
        // this is kinda unsafe but should work as long as i dont fuck it up
        private static readonly Vector3 InvalidTarget = new (float.MinValue, float.MinValue, float.MinValue);

        public string presetName;
        
        public SeekerComponent seeker;
        public ComputerComponent computer;
        public WarheadComponent warhead;
        public BoosterComponent booster;
        public AvionicsComponent avionics;

        public Rigidbody parentRigidbody;
        public Transform seekerHead;
        
        public bool hasLock { get; private set; }
        public bool selected { get; private set; }

        [SerializeField] private AeroBody aeroBody;
        [SerializeField] private float armTimeSeconds;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private AnimationCurve overloadControlCurve;

        private Transform _targetTransform;
        private Transform _transform;
        private Vector3 _lastTargetPosition = InvalidTarget;
        private Vector3 _lastVelocity;

        private const float InverseGrav = 1 / 9.8f;
        private bool _fired;
        private float _currentArmTime;
        private float _currentBurnTime;
        private bool _armed;
        
        private void Awake()
        {
            _transform = transform;
            
            aeroBody.rb.isKinematic = true;
            aeroBody.rb.detectCollisions = false;
        }

        public void Unlock()
        {
            _targetTransform = null;
            hasLock = false;
            seekerHead.localRotation = Quaternion.identity;
        }

        public void Select()
        {
            selected = true;
            hasLock = false;
            _targetTransform = null;
        }
        
        public void Deselect()
        {
            selected = false;
            hasLock = false;
            _targetTransform = null;
            seekerHead.localRotation = Quaternion.identity;
        }

        public void Fire()
        {
            _fired = true;
            selected = false;
            transform.SetParent(null);
            aeroBody.enabled = true;
            aeroBody.rb.isKinematic = false;
            aeroBody.rb.velocity = parentRigidbody.velocity;

            foreach (ParticleSystem system in particleSystems)
                system.Play();
        }

        public void Arm()
        {
            Debug.Log("ARMED");
            _armed = true;
            aeroBody.rb.detectCollisions = true;
        }

        private void FixedUpdate()
        {
            
            if (!selected && !_fired) return;
            
            
            if (_targetTransform != null)
                seeker.LookAt(seekerHead, _targetTransform.position, _transform.forward, _fired);
            else if (selected)
                seekerHead.localRotation = Quaternion.identity;
            
            _targetTransform = seeker.GetTargetPosition(seekerHead);

            hasLock = _targetTransform != null;
            
            if (!_fired) return;
            // only do the following if the missile has been fired

            if (!_armed && _currentArmTime < armTimeSeconds)
                _currentArmTime += Time.fixedDeltaTime;
            else if (!_armed && _currentArmTime >= armTimeSeconds)
                Arm();

            // i dont really like this all that much
            bool didApplyThrust = booster.ApplyThrust(aeroBody.rb, _currentBurnTime);
            if (!didApplyThrust && particleSystems[0].isPlaying)
                foreach (ParticleSystem system in particleSystems)
                    system.Stop();
            else if (didApplyThrust)
                _currentBurnTime += Time.fixedDeltaTime;
            
            if (!_armed) return;
            
            if (warhead.CheckExplode(_transform.position)) // TODO: Make not blow up on player / self and change prefab back to target layer
                Destroy(gameObject);
            

            if (hasLock)
            {
                Vector3 targetVelocity = (_targetTransform.position - _lastTargetPosition) * Util.InverseFDeltaTime;
                Vector3 direction = computer.GetDesiredDirection(_transform.position, aeroBody.rb.velocity,
                    (aeroBody.rb.velocity - _lastVelocity) * Util.InverseFDeltaTime,
                    _targetTransform.position, targetVelocity);

                if (!Util.VectorIsNan(direction)) 
                    aeroBody.SetControl(ControlToDirection(direction));

            }
            else
                aeroBody.SetControl(Vector3.zero);
            
                
            _lastTargetPosition = _targetTransform?.position ?? InvalidTarget;
            _lastVelocity = aeroBody.rb.velocity;
        }

        private float _maxGees;
        
        private Vector3 ControlToDirection(Vector3 forward)
        {
            float pitch = Util.AngleAroundAxis(forward, _transform.forward, _transform.right);
            float yaw = Util.AngleAroundAxis(forward, _transform.forward, _transform.up);

            // control should be zero at max overload and 1 at 0
            // this is not really a good way of doing this because its artificially limiting
            Vector3 acceleration = (aeroBody.rb.velocity - _lastVelocity) * Util.InverseFDeltaTime;
            float geeForce = Vector3.ProjectOnPlane(acceleration, _transform.forward).magnitude * InverseGrav;
            float controlCoefficient = -0.05f * overloadControlCurve.Evaluate(geeForce / avionics.maxOverloadGees); // TODO: should probably be damped using the pitch and yaw diffs

            if (geeForce > _maxGees)
                _maxGees = geeForce;
            // Debug.Log($"{geeForce}G, cc: {controlCoefficient}, Max G force: {_maxGees}");
            
            return new Vector3(pitch * controlCoefficient, yaw * controlCoefficient, 0);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_fired) return;
            
            warhead.Explode(_transform.position);
            Destroy(gameObject);
        }
        
    }
}
