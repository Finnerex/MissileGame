using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "OmniscientSeeker", menuName = "MissileComponent/OmnicientSeeker")]
    public class OmniscientSeeker : SeekerComponent
    {
        public override Transform GetTargetPosition(Transform thisTransform)
        {
            int numberOfObjects = OverlapConeNonAlloc(thisTransform.position, thisTransform.forward, HitObjects, fieldOfView * 0.5f, lockRange,
                LayerMask.GetMask("Target"));

            return numberOfObjects > 0 ? HitObjects[0].transform : null;
        }
    }
}