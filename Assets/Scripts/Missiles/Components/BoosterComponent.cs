using System;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Booster", menuName = "MissileComponent/Booster")]
    public class BoosterComponent : MissileComponent
    {
        public float burnTimeSeconds;
        public float thrustMultiplier;

        public bool ApplyThrust(Rigidbody rb, float currentBurnTime) // should be called in fixed update
        {
            if (currentBurnTime > burnTimeSeconds)
                return false;

            rb.AddForce(rb.transform.forward * thrustMultiplier);

            return true;
        }

        public override string ToString()
        {
            return $"{base.ToString()}\nBurn Time: {burnTimeSeconds}s\nThrust: {thrustMultiplier}";
        }
    }
}