using System;
using UnityEngine;

namespace Targeting
{
    public class TemperatureVolume : MonoBehaviour
    {
        public float temperature;

        [SerializeField] private float rearAspectAngle = 50; // rear aspect seekers should have a threshold of about 1500K
        [SerializeField] private float sideAspectAngle = 360 - 90; // side aspect about 150K

        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        public float GetPerceivedTempFromPosition(Vector3 position)
        {
            Vector3 displacement = position - _transform.position;
            float perceivedTemp = temperature / Mathf.Sqrt(displacement.magnitude); // slow ass double square root
            
            float angle = Vector3.Angle(-_transform.forward, displacement);
            if (angle < rearAspectAngle) // high key made this up
                perceivedTemp *= 100;
            else if (angle < sideAspectAngle)
                perceivedTemp *= 10;

            return perceivedTemp;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawFrustum(transform.position, sideAspectAngle, 100, 0, 1);
            
            Gizmos.matrix = Matrix4x4.Rotate(Quaternion.Euler(new Vector3(0, 180, 0)));
            Gizmos.color = Color.red;
            Gizmos.DrawFrustum(transform.position, rearAspectAngle, 100, 0, 1);
        }
    }
}