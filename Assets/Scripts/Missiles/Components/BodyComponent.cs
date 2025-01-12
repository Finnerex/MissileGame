using System;
using UnityEngine;

namespace Missiles.Components
{
    [CreateAssetMenu(fileName = "Body", menuName = "MissileComponent/Body")]
    public class BodyComponent : MissileComponent
    {
        public GameObject prefab; // object oriented programming brain rot lmfao
    }
    
}