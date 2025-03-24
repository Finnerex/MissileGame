using System;
using UnityEngine;

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

        public PIDController(Vector3 kp, Vector3 ki, Vector3 kd)
        {
            this.kp = kp;
            this.ki = ki;
            this.kd = kd;
            _integral = Vector3.zero;
            _lastError = Vector3.zero;
        }

        public Vector3 Update(Vector3 error, float deltaTime)
        {
            // Calculate integral and derivative terms
            _integral += error * deltaTime;
            Vector3 derivative = (error - _lastError) / deltaTime;

            // Calculate output
            Vector3 output = Mul(kp ,error) + Mul(ki, _integral) + Mul(kd, derivative);

            // Save current error for next derivative calculation
            _lastError = error;

            return output;
        }

        private Vector3 Mul(Vector3 a, Vector3 b) => new (a.x * b.x, a.y * b.y, a.z * b.z);

    }
}