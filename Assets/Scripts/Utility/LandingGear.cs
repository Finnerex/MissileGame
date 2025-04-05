using UnityEngine;

namespace Utility
{
    // making my own because idk how to use wheel collider right (it was made poorly)
    public class LandingGear : MonoBehaviour
    {

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform wheel;
        [SerializeField] private float wheelRadius;
        [SerializeField] private float height = 2;
        [SerializeField] private float springCoefficient = 0.1f;
        [SerializeField] private float dampening = 2f;

        [SerializeField] private float wheelFriction = 0.2f;
        [SerializeField] private float breakingFriction = 1.5f;
        [SerializeField] private float sidewaysFriction = 500;

        // private float _currentHeight;
        private Transform _transform;
        private float _lastDistance;

        private void Start()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            
            if (!Physics.Raycast(_transform.position, -_transform.up, out RaycastHit hit, height))
                return;

            // spring force
            float distFromCenter = (hit.point - _transform.position).magnitude - height;
            float damping = (distFromCenter - _lastDistance) * dampening * Util.InverseFDeltaTime;
            
            rb.AddForceAtPosition((distFromCenter * springCoefficient + damping) * -_transform.up, _transform.position); // Fs = kx
            _lastDistance = distFromCenter;

            wheel.transform.position = hit.point + _transform.up * wheelRadius;
            
            // friction force
            float friction = Input.GetKey(KeyCode.B) ? breakingFriction : wheelFriction;
            Vector3 velocity = rb.velocity + Vector3.Cross(rb.angularVelocity, _transform.position - rb.worldCenterOfMass);
            
            float forwardSpeed = Vector3.Dot(velocity, transform.forward);
            float rightwardSpeed = Vector3.Dot(velocity, transform.right);

            // float fwdSpeedCoef = Mathf.Sign(forwardSpeed) * Mathf.Sqrt(Mathf.Abs(forwardSpeed));
            rb.AddForceAtPosition(/*fwdSpeedCoef*/Mathf.Sign(forwardSpeed) * 0.001f * friction * -_transform.forward, _transform.position);
            rb.AddForceAtPosition(rightwardSpeed * 0.001f * sidewaysFriction * -_transform.right, _transform.position);
            
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * height);
        }
    }
}