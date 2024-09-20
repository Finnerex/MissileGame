using System;
using System.Linq;
using System.Linq.Expressions;
using Aerodynamics;
using Missiles.Components;
using UnityEngine;
using Utility;

namespace Missiles
{
    public class Missile : MonoBehaviour
    {
        // this is kinda unsafe but should work as long as i dont fuck it up
        public static readonly Vector3 InvalidTarget = new (float.MinValue, float.MinValue, float.MinValue);
        
        public SeekerComponent seeker;
        public ComputerComponent computer;
        public WarheadComponent warhead;
        public BoosterComponent booster;
        public AvionicsComponent avionics;

        public Rigidbody parentRigidbody;

        [SerializeField] private AeroBody aeroBody;
        [SerializeField] private Transform seekerHead;
        [SerializeField] private float armTimeSeconds;
        [SerializeField] private ParticleSystem[] particleSystems;

        private Transform _targetTransform;
        private Transform _transform;
        private Vector3 _lastTargetPosition = InvalidTarget;
        private Vector3 _lastVelocity;
        private float _lastLos;
        private float _lastLosRate;

        private ForwardUiIcon _seekerIcon;
        
        private const float InverseGrav = 1 / 9.8f;
        private bool _fired;
        private bool _selected;
        private float _currentArmTime;
        private float _currentBurnTime;
        private bool _armed;
        
        private void Awake()
        {
            _transform = transform;
            _seekerIcon = seekerHead.GetComponent<ForwardUiIcon>();

            aeroBody.rb.isKinematic = true;
            aeroBody.rb.detectCollisions = false;
        }

        private void Start()
        {
            _seekerIcon.icon = UIController.Instance.MissileLockRect;
            GetComponent<ForwardUiIcon>().icon = UIController.Instance.missileGimbalCircle;
        }

        public void Fire()
        {
            _fired = true;
            _selected = false;
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

        // TODO: pull out ui stuff to somewhere else because this is going to be used in a place that isn't just for player
        public void Select()
        {
            _selected = true;
            
            float circleDiameter = 2 * Mathf.Tan(seeker.fieldOfView * 0.5f * Mathf.Deg2Rad) * 500;
            UIController.Instance.MissileLockRect.sizeDelta = new Vector2(circleDiameter, circleDiameter);

            if (seeker.isCaged)
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(false);
            else
            {
                UIController.Instance.missileGimbalCircle.gameObject.SetActive(true);
                circleDiameter = 2 * Mathf.Tan((seeker.maxGimbalAngle + seeker.fieldOfView) * 0.5f * Mathf.Deg2Rad) * 500;
                UIController.Instance.missileGimbalCircle.sizeDelta = new Vector2(circleDiameter, circleDiameter);
            }
            
        }
        
        // public void UpdateCircles(float distance)
        // {
        //     _seekerIcon.distance = Mathf.Clamp(distance, 0, 500);
        // }

        private void FixedUpdate()
        {
            
            if (!_selected && !_fired) return;
            
            if (_targetTransform is not null)
                seeker.LookAt(seekerHead, _targetTransform.position, _transform.forward, _fired);
            
            _targetTransform = seeker.GetTargetPosition(seekerHead);

            // TODO: add back the selected check
            if (/*_selected && */_targetTransform is not null)
            {
                UIController.Instance.missileLockCircle.color = new Color(0.8f, 0.28f, 0.2f);
                // UpdateCircles((_transform.position - _targetTransform.position).magnitude);
                // seeker.LookAt(seekerHead, _targetTransform.position, _transform.forward, _fired);
            }
            else/* if (_selected)*/
            {
                UIController.Instance.missileLockCircle.color = new Color(0.75f, 0.75f, 0.7f);
                // UpdateCircles(seeker.lockRange);
                if (_selected)
                    seekerHead.localRotation = Quaternion.identity;
            }
            
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
            
            if (warhead.CheckExplode(_transform.position))
                Destroy(gameObject);
            

            if (_targetTransform is not null)
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
        
        private Vector3 ControlToDirection(Vector3 forward)
        {
            float pitch = Util.AngleAroundAxis(forward, _transform.forward, _transform.right);
            float yaw = Util.AngleAroundAxis(forward, _transform.forward, _transform.up);

            // control should be zero at max overload and 1 at 0
            // this is not really a good way of doing this
            Vector3 acceleration = (aeroBody.rb.velocity - _lastVelocity) * Util.InverseFDeltaTime;
            float geeForce = Vector3.ProjectOnPlane(acceleration, _transform.forward).magnitude * InverseGrav;
            float controlCoefficient = -0.04f * Mathf.Clamp((avionics.maxOverloadGees - geeForce) / avionics.maxOverloadGees, 0, 1);
            
            // Debug.Log($"Gees: {geeForce}, max: {avionics.maxOverloadGees}, control vector: {pitch}, {yaw}");

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
