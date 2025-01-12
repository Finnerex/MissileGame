using System;
using UnityEngine;

namespace Missiles.Components
{
    public abstract class MissileComponent : ScriptableObject
    {
        public string displayName;
        public int size; // small ass class !!!!! :(

        public override string ToString()
        {
            return $"Size: {size}u";
        }
    }
}