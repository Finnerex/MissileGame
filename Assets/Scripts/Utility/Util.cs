using UnityEngine;

namespace Utility
{
    public static class Util
    {

        public static readonly float InverseFDeltaTime = 1 / Time.fixedDeltaTime;
        
        public static float AngleAroundAxis(Vector3 to, Vector3 forward, Vector3 axis)
        {
            Vector3 right = Vector3.Cross(forward, axis);
            forward = Vector3.Cross(axis, right);
            
            return Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, forward)) * Mathf.Rad2Deg;
        }

        public static bool VectorIsNan(Vector3 v) => float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);

        public static Vector3 VectorClampComponents(Vector3 v, float min, float max) =>
            new (
                Mathf.Clamp(v.x, min, max),
                Mathf.Clamp(v.y, min, max),
                Mathf.Clamp(v.z, min, max)
            );

        // public static Quaternion InvertRotation(Quaternion rotation) =>
        //     rotation * Quaternion.AngleAxis(180, Vector3.up);

    }
}