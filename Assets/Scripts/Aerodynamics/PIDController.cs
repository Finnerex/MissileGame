using System;
using UnityEngine;
using Utility;

namespace Aerodynamics
{
    [Serializable]
    public class PIDController
    {
        [SerializeField] private Vector3 kp; // Proportional gain
        [SerializeField] private Vector3 ki; // Integral gain
        [SerializeField] private Vector3 kd; // Derivative gain

        private Vector3 _integral;
        private Vector3 _lastError;

        public Vector3 Update(Vector3 error, float deltaTime)
        {
            // Calculate integral and derivative terms
            _integral += error * deltaTime;
            Vector3 derivative = (error - _lastError) / deltaTime;

            // Calculate output
            Vector3 output = Util.VectorMultiplyComponents(kp ,error) +
                             Util.VectorMultiplyComponents(ki, _integral) +
                             Util.VectorMultiplyComponents(kd, derivative);

            // Save current error for next derivative calculation
            _lastError = error;

            return output;
        }
        

    }
}