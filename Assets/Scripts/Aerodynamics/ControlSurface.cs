using System;
using UnityEngine;

namespace Aerodynamics
{
    [Serializable]
    public struct ControlSurface
    {
        public GameObject obj;
        public ControlSurfaceType type;
        public bool invert;
        public float maxDeflectDegrees;
    }

    public enum ControlSurfaceType
    {
        Pitch,
        Roll,
        Yaw
    }
}