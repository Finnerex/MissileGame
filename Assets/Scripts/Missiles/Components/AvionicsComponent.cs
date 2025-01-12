using System;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Avionics", menuName = "MissileComponent/Avionics")]
    public class AvionicsComponent : MissileComponent
    {
        public GameObject prefab; // me when i repeat myself

        public float maxOverloadGees;

        public override string ToString()
        {
            return $"{base.ToString()}\nMax Overload: {maxOverloadGees}G";
        }
    }
}