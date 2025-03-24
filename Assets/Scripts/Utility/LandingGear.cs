using UnityEngine;

namespace Utility
{
    // making my own because idk how to use wheel collider right (it was made poorly)
    public class LandingGear : MonoBehaviour
    {

        [SerializeField] private Rigidbody rb;
        [SerializeField] private float height = 2;
        [SerializeField] private float springCoefficient = 0.1f;

        [SerializeField] private float wheelFriction = 0.2f;
        [SerializeField] private float breakingFriction = 1.5f;
        [SerializeField] private float sidewaysFriction = 500;

        // private float _currentHeight;
        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }

        private void FixedUpdate()
        {
            
            if (!Physics.Raycast(_transform.position, -_transform.up, out RaycastHit hit, height))
                return;

            // spring force
            // can ignore any projection that might be required because it will be on the line
            float distFromCenter = (hit.point - _transform.position).magnitude - height;
            // TODO: Damp
            rb.AddForceAtPosition(distFromCenter * springCoefficient * -_transform.up, _transform.position); // Fs = kx
            
            // friction force
            // could use physic mat here but thats something i dont care about atm the moment
            // could do actual normal force calculation // TODO: this
            float friction = Input.GetKey(KeyCode.B) ? breakingFriction : wheelFriction;
            Vector3 velocity = rb.velocity + Vector3.Cross(rb.angularVelocity, _transform.position - rb.worldCenterOfMass);
            
            float forwardSpeed = Vector3.Dot(velocity, transform.forward);
            float rightwardSpeed = Vector3.Dot(velocity, transform.right);

            if (forwardSpeed > 0.001f)
                rb.AddForceAtPosition(Mathf.Sign(forwardSpeed) * friction * -_transform.forward, _transform.position);
            if (rightwardSpeed > 0.001f)
                rb.AddForceAtPosition(Mathf.Sign(rightwardSpeed) * sidewaysFriction * -_transform.right, _transform.position);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position - transform.up * height);
        }
    }
}