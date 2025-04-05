using System;
using Aerodynamics;
using Missiles;
using Targeting;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Enemy
{
    // maybe i should have split this into a few like i didi for the player one
    public class EnemyAI : MonoBehaviour
    {

        private enum State
        {
            Searching, 
            Tracking,
        }
        
        [SerializeField] private AeroBody aeroBody;
        [SerializeField] private Thruster engine;
        [SerializeField] private WeaponsSystem weaponsSystem;
        [SerializeField] private SpottingSystem spottingSystem;

        [SerializeField] private float spottingPeriodSeconds = 10;
        
        [Header("Random Search")]
        [SerializeField] private Vector2 cruisingAltitudeRange;
        [SerializeField] private Vector2 terrainSize;
        [SerializeField] private float squareSearchCheckpointDistance = 1000;
        [SerializeField] private float maxAngleDifference = 30;
        [SerializeField] private Vector2 distanceRange = new (50, 400);
        [SerializeField] private Vector2 altitudeDifferenceRange = new(-50, 50);

        [Header("Targeting")]
        [SerializeField] private float targetAttackRange = 5000;
        [SerializeField] private float targetDistanceToTarget = 500;
        
        private const float RollThreshold = 12;
        
        private State _aiState;
        private Vector3 _targetHeading;
        private Vector3 _targetPosition;
        private Transform _target;

        private float _currentSpotTime;

        private float _sqrAttackRange;

        private Transform _transform;

        private void Start()
        {
            _transform = transform;

            _targetPosition = NextRandomPosition();
            // _targetPosition.y = Random.Range(cruisingAltitudeRange.x, cruisingAltitudeRange.y);
            // _targetPosition.x = Random.Range(0, terrainSize.x);
            // _targetPosition.z = Random.Range(0, terrainSize.y);
            
            Debug.Log($"going to {_targetPosition}");

            engine.Throttle = 0.9f;
            aeroBody.rb.velocity = _transform.forward * 150;

            _sqrAttackRange = targetAttackRange * targetAttackRange;
        }

        private void Update()
        {
            switch (_aiState)
            {
                case State.Searching:
                    OnSearch();
                    break;
                case State.Tracking:
                    OnTrack();
                    break;
            }
            // maybe periodic flares idk
            // prime directive - dont hit the ground
            
            Vector3 displacement = transform.position - _targetPosition;
            
            // maybe do this less frequently?
            Vector3 control = Vector3.zero;
                
            float yawAngle = Util.AngleAroundAxis(_transform.forward, displacement, _transform.up);
            float pitchAngle = Util.AngleAroundAxis(_transform.forward, displacement, _transform.right) + 1;
            float rollAngle = Util.AngleAroundAxis(Vector3.down, -_transform.up, _transform.forward);
            
            // perchance dampen based on distance
            control.x = pitchAngle * -0.05f;
            control.y = yawAngle * 0.02f;
            control.z = yawAngle switch
            {
                > RollThreshold => yawAngle - RollThreshold,
                < -RollThreshold => yawAngle + RollThreshold,
                _ => -rollAngle / RollThreshold // auto level - should be somehow mixed with the other thing
            } * 0.02f;
            
            aeroBody.SetControl(control);

        }

        private void OnSearch()
        {
            // throttle should probably not be 100%
            // nonuniform circling flight pattern
            // wait for spotting system to have enemy
            // maybe vary cruising altitude
            
            // track this wander using only _targetPosition
            Vector3 displacement = transform.position - _targetPosition;

            if (displacement.sqrMagnitude <= squareSearchCheckpointDistance)
                _targetPosition = NextRandomPosition(); // should probably be some point not too far off of current direction, close ish by
            
            // spotting
            _currentSpotTime += Time.deltaTime;
            if (_currentSpotTime > spottingPeriodSeconds)
            {
                _currentSpotTime = 0;
                spottingSystem.TrySpot();

                _target = spottingSystem.GetClosest();
                if (_target is null) return;

                if ((_target.position - _transform.position).sqrMagnitude <= _sqrAttackRange) // among other positioning conditions
                    _aiState = State.Tracking;

            }
            
            // dont put important code here bc i may have returned
            
        }
        

        private Vector3 NextRandomPosition()
        {
            float distance = Random.Range(distanceRange.x, distanceRange.y);

            float angle = (-Util.AngleAroundAxis(Vector3.right, _transform.forward, Vector3.up) +
                           Random.Range(-maxAngleDifference, maxAngleDifference)) * Mathf.Deg2Rad;
            // TODO: better edging must be achieved - maybe dampen as gets closer to edge
            float x = Mathf.Clamp(_transform.position.x + Mathf.Cos(angle) * distance, 0, terrainSize.x);
            float z = Mathf.Clamp(_transform.position.z + Mathf.Sin(angle) * distance, 0, terrainSize.y);
            
            float y = Mathf.Clamp(_transform.position.y +Random.Range(altitudeDifferenceRange.x, altitudeDifferenceRange.y), cruisingAltitudeRange.x, cruisingAltitudeRange.y);

            return new Vector3(x, y, z);
        }
        

        private void OnTrack()
        {
            // making this thing actually dogfight would be a pain in the arse
            // should attempt to get behind the current target if using ir missiles
            // fire missile if locked
            // flare if enemy missile / pre-flare if enemy behind & just in general 
            // _targetPosition should be behind the enemy, _targetHeading should be towards the enemy

            _targetPosition = _target.position - _target.forward * targetDistanceToTarget;

            if ((_target.position - _transform.position).sqrMagnitude <= _sqrAttackRange) // among other positioning conditions
                _aiState = State.Searching;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new(0.05f, 0.8f, 0.95f, 0.7f);
            Gizmos.DrawSphere(_targetPosition, Mathf.Sqrt(squareSearchCheckpointDistance));
        }
    }
}