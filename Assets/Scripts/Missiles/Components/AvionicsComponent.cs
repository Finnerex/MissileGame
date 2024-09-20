using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Avionics", menuName = "MissileComponent/Avionics")]
    public class AvionicsComponent : MissileComponent
    {
        public float maxOverloadGees;
    }
}