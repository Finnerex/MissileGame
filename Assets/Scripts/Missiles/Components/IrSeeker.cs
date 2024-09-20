using Targeting;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "IrSeeker", menuName = "MissileComponent/IrSeeker")]
    public class IrSeeker : SeekerComponent
    {

        [SerializeField] private float minTemperature;
        
        public override Transform GetTargetPosition(Transform thisTransform)
        {
            Debug.Log("position is being gotten from ir seeker!!");
            
            int numberOfObjects = ConeCastNonAlloc(thisTransform.position, thisTransform.forward, HitObjects, fieldOfView * 0.5f, lockRange,
                LayerMask.GetMask("Target"));

            return FilterTarget(HitObjects, numberOfObjects);
        }

        private Transform FilterTarget(Collider[] targets, int numTargets)
        {
            TemperatureVolume highestTemp = null;
            
            for (int i = 0; i < numTargets; i++)
            {
                if (!targets[i].TryGetComponent(out TemperatureVolume volume))
                    continue;

                // TODO: make perceived temperature diminish with distance
                if ((highestTemp is null || volume.temperature > highestTemp.temperature) &&
                    volume.temperature > minTemperature)
                {
                    highestTemp = volume;
                    Debug.Log("highest temp is now: " + volume.temperature);
                }
            }

            return highestTemp?.transform;
        }
        
    }
}