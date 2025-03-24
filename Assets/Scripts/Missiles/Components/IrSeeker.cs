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
            Vector3 position = thisTransform.position;
            
            int numberOfObjects = OverlapConeNonAlloc(position, thisTransform.forward, HitObjects, fieldOfView * 0.5f, lockRange,
                LayerMask.GetMask("Target"));
            // TODO: I think i need a better solution for finding what to target because omniscient can target flares or whatever

            return FilterTarget(HitObjects, numberOfObjects, position);
        }

        private Transform FilterTarget(Collider[] targets, int numTargets, Vector3 position)
        {
            TemperatureVolume highestTempVolume = null;
            float highestPerceivedTemp = 0;
            
            for (int i = 0; i < numTargets; i++)
            {
                if (!targets[i].TryGetComponent(out TemperatureVolume volume))
                    continue;

                float perceivedTemp = volume.GetPerceivedTempFromPosition(position);
                
                if ((highestTempVolume is null || perceivedTemp > highestPerceivedTemp) && perceivedTemp > minTemperature)
                {
                    highestTempVolume = volume;
                    highestPerceivedTemp = perceivedTemp;
                }
            }

            return highestTempVolume?.transform;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nSeeker Type: IR\nTemperature Threshold: {minTemperature}K";
        }
    }
}