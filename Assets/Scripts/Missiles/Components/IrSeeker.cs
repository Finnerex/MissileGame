using Targeting;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "IrSeeker", menuName = "MissileComponent/IrSeeker")]
    public class IrSeeker : SeekerComponent
    {

        [SerializeField] private float minTemperature;
        
        // TODO: Make this see the sun
        public override Transform GetTargetPosition(Transform thisTransform)
        {
            // Debug.Log("position is being gotten from ir seeker!!");

            Vector3 position = thisTransform.position;
            
            int numberOfObjects = OverlapConeNonAlloc(position, thisTransform.forward, HitObjects, fieldOfView * 0.5f, lockRange,
                LayerMask.GetMask("Target"));

            return FilterTarget(HitObjects, numberOfObjects, position);
        }

        private Transform FilterTarget(Collider[] targets, int numTargets, Vector3 position)
        {
            TemperatureVolume highestTemp = null;
            float highestPerceivedTemp = 0;
            
            for (int i = 0; i < numTargets; i++)
            {
                if (!targets[i].TryGetComponent(out TemperatureVolume volume))
                    continue;

                float perceivedTemp = volume.temperature / (position - volume.transform.position).sqrMagnitude;

                // TODO: make perceived temperature diminish with distance
                if ((highestTemp is null || perceivedTemp > highestPerceivedTemp) && perceivedTemp > minTemperature)
                {
                    highestTemp = volume;
                    highestPerceivedTemp = perceivedTemp;
                    // Debug.Log("highest temp is now: " + volume.temperature);
                }
            }

            return highestTemp?.transform;
        }
        
    }
}