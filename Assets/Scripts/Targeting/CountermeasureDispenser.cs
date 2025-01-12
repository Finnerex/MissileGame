using System;
using System.Collections;
using UnityEngine;

namespace Targeting
{
    public class CountermeasureDispenser : MonoBehaviour
    {
        [SerializeField] private Rigidbody parentRigidbody;
        
        [SerializeField] private Rigidbody flarePrefab;
        [SerializeField] private float flareDeployForce = 10;
        private Transform _transform;

        private void Start()
        {
            _transform = transform;
        }
        
        public void DeployFlare()
        {
            Rigidbody createdFlare = Instantiate(flarePrefab, _transform.position, Quaternion.identity);
            createdFlare.velocity = parentRigidbody.velocity;
            createdFlare.AddForce(_transform.forward * flareDeployForce);
            Destroy(createdFlare.gameObject, 5);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }
    }
}