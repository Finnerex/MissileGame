using System;
using UnityEngine;

namespace Missiles.Components
{
    public abstract class MissileComponent : ScriptableObject
    {
        public GameObject prefab;
        public int size;
        [NonSerialized] public GameObject SpawnedObject; // TODO: this is not going to work if there are multiple but might be fine
    }
}