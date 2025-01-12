using System;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Computer", menuName = "MissileComponent/Computer")]
    public class ComputerComponent : MissileComponent
    {
        [SerializeField] private ComputerGuidanceType guidanceType;
        public bool hasInertialGuidance;
        [SerializeField] private float proportionalityConstant = 500;
        // maybe do seeker support
        
        public Vector3 GetDesiredDirection(Vector3 position, Vector3 velocity, Vector3 acceleration, Vector3 targetPosition, Vector3 targetVelocity)
        {
            // calculate new control vector
            Vector3 direction = Vector3.zero;

            switch (guidanceType)
            {
                case ComputerGuidanceType.Direct: // maybe include last target pos == invalid target
                    direction = targetPosition - position; // line of sight
                    break;
                
                case ComputerGuidanceType.PositionExtrapolated: 
                    float timeToCollision = TimeToCollision(position - targetPosition, velocity - targetVelocity);
                    Vector3 extrapolatedTargetPosition = ExtrapolatePosition(targetPosition, targetVelocity, Vector3.zero, timeToCollision);
                    direction = extrapolatedTargetPosition - position;/* GetLineOfSight(position, extrapolatedTargetPosition);*/
                    break;
                
                case ComputerGuidanceType.ProportionalNavigation:
                    Vector3 desiredAcceleration = GetProportionalAcceleration(position - targetPosition, velocity - targetVelocity)
                    + acceleration;
                    direction = velocity - desiredAcceleration * Time.deltaTime; // desired velocity
                    // direction.z *= -1;
                    break;
            }

            return direction;

        }
        
        private float TimeToCollision(Vector3 displacement, Vector3 relativeVelocity)
        {
            if (relativeVelocity == Vector3.zero)
                return 0;
            
            return -Vector3.Dot(displacement, relativeVelocity) / relativeVelocity.sqrMagnitude;
        }

        private Vector3 ExtrapolatePosition(Vector3 position, Vector3 velocity, Vector3 acceleration, float time) =>
            position + velocity * time + acceleration * (0.5f * time * time);
        
        private Vector3 GetProportionalAcceleration(Vector3 displacement, Vector3 relativeVelocity) =>
            proportionalityConstant * Vector3.Cross(relativeVelocity, Vector3.Cross(displacement, relativeVelocity) / displacement.sqrMagnitude);

        public override string ToString()
        {
            return $"{base.ToString()}\nGuidance Type: {guidanceType}";
        }
    }

    public enum ComputerGuidanceType
    {
        Direct,
        PositionExtrapolated,
        ProportionalNavigation,
    }
}
