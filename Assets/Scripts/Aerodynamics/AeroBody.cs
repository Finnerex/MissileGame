using System;
using UnityEngine;
using Utility;

namespace Aerodynamics
{
    public class AeroBody : MonoBehaviour
    {
        [SerializeField] private float aerodynamicDrag = 1;
        
        [SerializeField] private ControlSurface[] controlSurfaces;
        [SerializeField] private AeroSurface[] aeroSurfaces;

        // Likely have this handled in an environment manager object
        public float airDensity = 1.225f;
        public Rigidbody rb;

        private Vector3 _lastTorque;

        private const float PredictionStep = 0.3f;
        

        private void FixedUpdate()
        {
            BiVector3 forceAndTorqueThisFrame = CalculateAerodynamicForces(rb.velocity, rb.angularVelocity, rb.worldCenterOfMass);

            Vector3 velocityPrediction = PredictVelocity(rb.GetAccumulatedForce());
            Vector3 angularVelocityPrediction = PredictAngularVelocity(forceAndTorqueThisFrame.q);

            BiVector3 forceAndTorquePrediction = CalculateAerodynamicForces(velocityPrediction, angularVelocityPrediction, rb.worldCenterOfMass);
            
            BiVector3 currentForceAndTorque = (forceAndTorqueThisFrame + forceAndTorquePrediction) * 0.5f;
            rb.AddForce(currentForceAndTorque.p);
            rb.AddTorque(currentForceAndTorque.q);

            Debug.DrawLine(transform.position, transform.position + currentForceAndTorque.p * 0.1f);

        }
        
        private BiVector3 CalculateAerodynamicForces(Vector3 velocity, Vector3 angularVelocity, Vector3 centerOfMass)
        {
            BiVector3 forceAndTorque = new BiVector3();
            foreach (var surface in aeroSurfaces)
            {
                if (surface == null) continue;
                
                Vector3 relativePosition = surface.transform.position - centerOfMass;
                forceAndTorque += surface.CalculateForces(velocity + Vector3.Cross(angularVelocity, relativePosition),
                    airDensity, relativePosition);
            }

            forceAndTorque.p -= aerodynamicDrag * rb.velocity.magnitude * rb.velocity; // Cv^2 
            
            return forceAndTorque;
        }
        
        private Vector3 PredictVelocity(Vector3 force)
        {
            return rb.velocity + Time.fixedDeltaTime * PredictionStep * force / rb.mass;
        }
        
        private Vector3 PredictAngularVelocity(Vector3 torque)
        {
            Quaternion inertiaTensorWorldRotation = rb.rotation * rb.inertiaTensorRotation;
            Vector3 torqueInDiagonalSpace = Quaternion.Inverse(inertiaTensorWorldRotation) * torque;
            Vector3 angularVelocityChangeInDiagonalSpace;
            
            angularVelocityChangeInDiagonalSpace.x = torqueInDiagonalSpace.x / rb.inertiaTensor.x;
            angularVelocityChangeInDiagonalSpace.y = torqueInDiagonalSpace.y / rb.inertiaTensor.y;
            angularVelocityChangeInDiagonalSpace.z = torqueInDiagonalSpace.z / rb.inertiaTensor.z;

            return rb.angularVelocity + Time.fixedDeltaTime * PredictionStep * (inertiaTensorWorldRotation * angularVelocityChangeInDiagonalSpace);
        }


        // Vector is in PITCH, YAW, ROLL as in x, y, z intended rotation
        public void SetControl(Vector3 control)
        {
            SetControl(control.x, control.y, control.z);
        }
        
        public void SetControl(float pitch, float yaw, float roll)
        {
            pitch = Mathf.Clamp(pitch, -1, 1);
            roll = Mathf.Clamp(roll, -1, 1);
            yaw = Mathf.Clamp(yaw, -1, 1);
            
            foreach (ControlSurface surface in controlSurfaces)
            {
                if (surface.obj is null) continue;
                
                Vector3 oldRotation = surface.obj.transform.localEulerAngles;
                
                Vector3 rotation = new Vector3(surface.type switch
                {
                    ControlSurfaceType.Pitch => pitch,
                    ControlSurfaceType.Roll => roll,
                    ControlSurfaceType.Yaw => yaw,
                    _ => throw new Exception("kys")
                } * surface.maxDeflectDegrees * (surface.invert ? -1 : 1), oldRotation.y, oldRotation.z);

                surface.obj.transform.localEulerAngles = rotation;

            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 1, 1);
            Gizmos.DrawSphere(transform.position + CalculateColOffset(), 0.4f);
            Gizmos.color = new Color(1, 0.92f, 0.016f, 1);
            Gizmos.DrawSphere(rb.worldCenterOfMass, 0.4f);
        }

        private Vector3 CalculateColOffset()
        {
            Vector3 distanceWeightedAreaSum = Vector3.zero;
            float areaSum = 0;
            
            foreach (AeroSurface surface in aeroSurfaces)
            {
                areaSum += surface.Area;
                distanceWeightedAreaSum += surface.Area * (surface.transform.position - transform.position); // maybe should be from com but might not matter
            }

            return distanceWeightedAreaSum / areaSum;
        }
    }
}
