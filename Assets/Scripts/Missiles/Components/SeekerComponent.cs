using System;
using UnityEngine;

namespace Missiles.Components
{
    public abstract class SeekerComponent : MissileComponent
    {
        
        public bool isCaged; // doesnt have big circle
        public float fieldOfView; // size of small circle
        public float maxGimbalAngle; // size of big circle
        public float lockRange;
        
        // dont know if this will work; generally bad idea to store data during run in scriptable object but its static anyway
        // seems to work
        [NonSerialized] protected static readonly Collider[] HitObjects = new Collider[20];
        
        public abstract Transform GetTargetPosition(Transform thisTransform);

        // TODO: Find a better solution for a cone cast
        // this one will end up just looking at every object on one side of the player as distance gets larger
        public static int OverlapConeNonAlloc(Vector3 origin, Vector3 direction, Collider[] result, float halfAngle, float distance, int layerMask)
        {
            int size = Physics.OverlapSphereNonAlloc(origin + direction * (distance * 0.5f), distance * 0.5f, result, layerMask);

            int outSize = 0;
            for (int i = 0; i < size; i++)
            {
                float angleToTarget = Vector3.Angle(direction, result[i].transform.position - origin);
                
                // Debug.Log($"angle to target {i}: {angleToTarget}");

                if (angleToTarget <= halfAngle && !Physics.Linecast(origin, result[i].transform.position, ~layerMask))
                {
                    result[outSize] = result[i];
                    outSize++;
                }
                else
                    result[i] = null;

            }

            return outSize;

        }

        public void LookAt(Transform transform, Vector3 at, Vector3 forward, bool fired)
        {
            if (!fired && isCaged)
                return;
            
            Vector3 displacement = at - transform.position;
            float angle = Vector3.Angle(forward, displacement);
            
            if (angle > maxGimbalAngle)
                return;
                
            transform.LookAt(at);
        }

        private void OnValidate()
        {
            if (isCaged)
                maxGimbalAngle = fieldOfView;
        }
    }
}