using System;
using UnityEngine;
using Utility;

namespace Aerodynamics
{
    public class AeroSurface : MonoBehaviour
    {
        [SerializeField] private float area = 2f;

        private Transform _transform;

        public float aoa { get; private set; }
        public float Area => area;
        
        private void Start()
        {
            _transform = transform;
        }
        

        public BiVector3 CalculateForces(Vector3 velocity, float airDensity, Vector3 relativePosition)
        {
            velocity *= Time.fixedDeltaTime;
            
            // float AoA = Vector3.SignedAngle(_transform.forward, velocity, _transform.right);
            float AoA = Util.AngleAroundAxis(_transform.forward, velocity, _transform.right);
            
            if (AoA > 90)
                AoA = 180 - AoA;
            else if (AoA < -90)
                AoA = -180 - AoA;

            aoa = AoA;
            
            float dynamicPressure = 0.5f * airDensity * velocity.sqrMagnitude;
            float forceMagnitude = dynamicPressure * area * Mathf.PI * (AoA * Mathf.Deg2Rad);

            BiVector3 forceAndTorque = new BiVector3();
            
            forceAndTorque.p += forceMagnitude * _transform.up;
            forceAndTorque.q += Vector3.Cross(relativePosition, forceAndTorque.p);
            // forceAndTorque.q += -_transform.forward * (dynamicPressure * area * (area * 0.5f));
            
            Debug.DrawLine(_transform.position, transform.position + (forceMagnitude * 0.1f) * _transform.up);
            

            return forceAndTorque;
        }


        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay(transform.position, transform.up);
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawRay(transform.position, transform.right);
        // }
    }
}